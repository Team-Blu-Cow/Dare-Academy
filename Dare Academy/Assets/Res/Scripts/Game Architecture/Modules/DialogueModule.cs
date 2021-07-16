using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DialogueEditor;

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

            _currentConversation = in_conversation.GetComponent<NPCConversation>();
            _dialogueCanvas = Instantiate(Resources.Load<GameObject>("prefabs/DialogueCanvas"));
            _dialogueCanvas.name = "Dialogue Canvas";
            //_ContinueButton = GameObject.Find("CanvasManager").transform.Find("ContinueButton").gameObject;
            _ContinueButton = ConversationManager.Instance.DialoguePanel.transform.Find("ContinueButton").gameObject;
            _ContinueButton.SetActive(false);
            _canvasAnimation = _dialogueCanvas.GetComponentInChildren<Animator>();
            App.GetModule<InputModule>().PlayerController.Disable();                // stop all input other than dialogue
            App.GetModule<InputModule>().SystemController.Disable();                //
            App.GetModule<InputModule>().DialogueController.Enable();               //

            if (App.CanvasManager == null)
            {
                Debug.LogWarning("[App/DialogueModule] could not find canvas manager");
                return;
            }

            App.CanvasManager.AddCanvas(_dialogueCanvas);
            App.CanvasManager.OpenCanvas(App.CanvasManager.GetCanvasContainer("Dialogue Canvas"), true);
            ConversationManager.Instance.StartConversation(_currentConversation);

            StartCoroutine(ProgressDialogue());
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
            App.GetModule<InputModule>().DialogueController.Disable();
            App.GetModule<InputModule>().PlayerController.Enable();
            App.GetModule<InputModule>().SystemController.Enable();
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