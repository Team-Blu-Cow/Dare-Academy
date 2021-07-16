using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DialogueEditor;
using System.Threading.Tasks;

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

            InputModule input = App.GetModule<InputModule>();

            input.PlayerController.Disable();                // stop all input other than dialogue
            input.SystemController.Disable();                //
            input.DialogueController.Enable();               //

            if (App.CanvasManager == null)
            {
                Debug.LogWarning("[App/DialogueModule] could not find canvas manager");
                return;
            }

            if (in_conversation != null)
            {
                StartCoroutine(_StartDialogue(in_conversation));
            }
        }

        private IEnumerator _StartDialogue(GameObject in_conversation)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("prefabs/DialogueCanvas");

            while (!request.isDone)
                yield return null;

            yield return null;
            _dialogueCanvas = Instantiate(request.asset as GameObject);
            _dialogueCanvas.name = "Dialogue Canvas";
            yield return null;

            _canvasAnimation = _dialogueCanvas.GetComponentInChildren<Animator>();

            _ContinueButton = ConversationManager.Instance.DialoguePanel.transform.Find("ContinueButton").gameObject;
            _ContinueButton.SetActive(false);

            _currentConversation = in_conversation.GetComponent<NPCConversation>();
            Task<bool> task = Task.Run(() => ConversationManager.Instance._DeserializeConversation(_currentConversation));

            if (!task.IsCompleted)
                yield return null;

            yield return null;
            ConversationManager.Instance._DeserializeConversation(_currentConversation);

            App.CanvasManager.AddCanvas(_dialogueCanvas);
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
            _canvasAnimation.SetTrigger("FadeOut");
            yield return new WaitForSeconds(_fadeDelay);
            App.CanvasManager.RemoveCanvasContainer("Dialogue Canvas");
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

        private void Start()
        {
            App.GetModule<InputModule>().DialogueController.Dialogue.Cancel.performed += StopDialogue;
            App.GetModule<InputModule>().DialogueController.Dialogue.NextOption.performed += NextOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.PreviousOption.performed += PreviousOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Select.performed += SelectOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed += SkipDialogue;
        }

        private void OnDestroy()
        {
            App.GetModule<InputModule>().DialogueController.Dialogue.Cancel.performed -= StopDialogue;
            App.GetModule<InputModule>().DialogueController.Dialogue.NextOption.performed -= NextOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.PreviousOption.performed -= PreviousOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Select.performed -= SelectOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed -= SkipDialogue;
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