using System.Collections;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using DialogueEditor;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace blu
{
    public class DialogueModule : Module
    {
        private float _fadeDelay = 0.5f;
        private float _skipTimer = 0f;
        private bool _skippable = false;
        private bool _closable = false;

        private GameObject _dialogueCanvas;
        private Animator _canvasAnimation;
        private NPCConversation _currentConversation;
        public GameObject _EventSystem;
        private GameObject _ContinueButton;
        private InputModule _input;
        private GameObject _dialogueCanvasPrefab;
        private int _countActiveDialogue = 0;

        public static event Action DialogueFinished;

        public bool DialogueActive => _countActiveDialogue != 0;

        //private Image _continueButton;
        public GameObject EventSystem { set => _EventSystem = value; }

        public void StartDialogue(GameObject in_conversation = null)
        {
            if (in_conversation == null)
                return;

            if (_EventSystem != null)
            {
                _EventSystem.SetActive(false);
            }

            _input.PlayerController.Disable();                // stop all input other than dialogue
            _input.SystemController.Disable();                //
            _input.DialogueController.Enable();               //

            if (App.CanvasManager == null)
            {
                Debug.LogWarning("[App/DialogueModule] could not find canvas manager");

                _input.PlayerController.Enable();
                _input.SystemController.Enable();
                _input.DialogueController.Disable();

                return;
            }

            _countActiveDialogue++;
            StartCoroutine(_StartDialogue(in_conversation));
        }

        private IEnumerator _StartDialogue(GameObject in_conversation)
        {
            _dialogueCanvas.SetActive(true);

            if (_ContinueButton == null)
                _ContinueButton = ConversationManager.Instance.DialoguePanel.transform.Find("ContinueButton").gameObject;

            _ContinueButton.SetActive(false);

            _currentConversation = in_conversation.GetComponent<NPCConversation>();
            Task<bool> task = Task.Run(() => ConversationManager.Instance._DeserializeConversation(_currentConversation));

            while (!task.IsCompleted)
                yield return null;

            App.CanvasManager.OpenCanvas(App.CanvasManager.GetCanvasContainer("Dialogue Canvas"), true);

            ConversationManager.Instance._EnableUI();
            StartCoroutine(ProgressDialogue());

            yield return null;
        }

        private IEnumerator ProgressDialogue()
        {
            yield return new WaitForSeconds(_fadeDelay);
            _closable = true;

            while (ConversationManager.Instance.IsConversationActive)
            {
                if (ConversationManager.Instance.CurrentState == ConversationManager.eState.Idle)
                {
                    _ContinueButton.SetActive(true);
                }
                else
                {
                    _ContinueButton.SetActive(false);
                }
                yield return null;
            }

            StopDialogue();
        }

        private IEnumerator FadeOutCanvas()
        {
            if (_canvasAnimation == null)
                _canvasAnimation = _dialogueCanvas.GetComponentInChildren<Animator>();

            _canvasAnimation.SetTrigger("FadeOut");
            yield return new WaitForSeconds(_fadeDelay);
            App.CanvasManager.RemoveCanvasContainer("Dialogue Canvas", false);
            DialogueFinished?.Invoke();
            yield break;
        }

        public void StopDialogue()
        {
            if (_EventSystem != null)
            {
                _EventSystem.SetActive(true);
            }
            _closable = false;
            StartCoroutine(FadeOutCanvas());
            _dialogueCanvas.SetActive(false);

            _countActiveDialogue--;

            InputModule input = App.GetModule<InputModule>();
            input.DialogueController.Disable();
            input.PlayerController.Enable();
            input.SystemController.Enable();
        }

        private void StopDialogue(InputAction.CallbackContext context)
        {
            if (_closable)
            {
                ConversationManager.Instance.ClearOptions();
                ConversationManager.Instance.EndConversation();
            }
        }

        private void SkipDialogue(InputAction.CallbackContext context)
        {
            if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                if (ConversationManager.Instance.CurrentState == ConversationManager.eState.ScrollingText)
                {
                    if (_skippable)
                    {
                        ConversationManager.Instance.Skip = true;
                    }
                }
            }
        }

        private void NextOption(InputAction.CallbackContext context)
        {
            if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                ConversationManager.Instance.SelectNextOption();
            }
        }

        private void PreviousOption(InputAction.CallbackContext context)
        {
            if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                ConversationManager.Instance.SelectPreviousOption();
            }
        }

        private void SelectOption(InputAction.CallbackContext context)
        {
            if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                if (ConversationManager.Instance.CurrentState != ConversationManager.eState.ScrollingText)
                {
                    ConversationManager.Instance.PressSelectedOption();
                    _skipTimer = 0f;
                    _skippable = false;
                }
            }
        }

        private void OnSceneSwitch(Scene scene, LoadSceneMode mode)
        {
            InitDialogueCanvas();
        }

        private void InitDialogueCanvas()
        {
            if (_dialogueCanvas == null)
            {
                _dialogueCanvas = Instantiate(_dialogueCanvasPrefab);
                _dialogueCanvas.name = "Dialogue Canvas";
                GameObject.DontDestroyOnLoad(_dialogueCanvas);
                _dialogueCanvas.SetActive(false);
                App.CanvasManager.AddCanvas(_dialogueCanvas);

                _canvasAnimation = _dialogueCanvas.GetComponentInChildren<Animator>();
                _ContinueButton = ConversationManager.Instance.DialoguePanel.transform.Find("ContinueButton").gameObject;
            }
        }

        private void Start()
        {
            _input = App.GetModule<InputModule>();

            _input.DialogueController.Dialogue.Cancel.performed += StopDialogue;
            _input.DialogueController.Dialogue.NextOption.performed += NextOption;
            _input.DialogueController.Dialogue.PreviousOption.performed += PreviousOption;
            _input.DialogueController.Dialogue.Select.performed += SelectOption;
            _input.DialogueController.Dialogue.Skip.performed += SkipDialogue;
            SceneManager.sceneLoaded += OnSceneSwitch;

            _dialogueCanvasPrefab = Resources.Load<GameObject>("prefabs/UI prefabs/DialogueCanvas");
            InitDialogueCanvas();
        }

        private void OnDestroy()
        {
            _input.DialogueController.Dialogue.Cancel.performed -= StopDialogue;
            _input.DialogueController.Dialogue.NextOption.performed -= NextOption;
            _input.DialogueController.Dialogue.PreviousOption.performed -= PreviousOption;
            _input.DialogueController.Dialogue.Select.performed -= SelectOption;
            _input.DialogueController.Dialogue.Skip.performed -= SkipDialogue;
            SceneManager.sceneLoaded -= OnSceneSwitch;
            GameObject.Destroy(_dialogueCanvas);
        }

        private void Update()
        {
            _skipTimer += Time.deltaTime;

            if (_skipTimer > 0.25f)
            {
                _skippable = true;
            }
        }

        protected override void SetDependancies()
        {
            _dependancies.Add(typeof(InputModule));
        }

        public override void Initialize()
        {
            return;
        }
    }
}