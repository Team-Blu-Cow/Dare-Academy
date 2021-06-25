using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;

namespace blu
{
    public class DialogueModule : Module
    {
        private float _fadeDelay = 0.5f;
        private bool _closable = false;
        private bool _dialogueFinished;

        private GameObject _dialogueCanvas;
        private Animator _canvasAnimation;
        private NPCConversation _currentConversation;
        private GameObject _EventSystem;
        public GameObject EventSystem { set => _EventSystem = value; }

        public void StartDialogue(GameObject in_conversation = null)
        {
            if (in_conversation == null)
                return;

            //TODO @Anyone: Im aware this is slow but I have no way to
            // guarentee if an event system exists in any given scene
            //_EventSystem = GameObject.Find("EventSystem");

            if (_EventSystem != null)
            {
                _EventSystem.SetActive(false);
            }

            _currentConversation = in_conversation.GetComponent<NPCConversation>();
            _dialogueCanvas = Instantiate(Resources.Load<GameObject>("prefabs/DialogueCanvas"));
            _dialogueCanvas.name = "Dialogue Canvas";
            _canvasAnimation = _dialogueCanvas.GetComponentInChildren<Animator>();
            App.GetModule<InputModule>().PlayerController.Disable();                // stop all input other than dialogue
            App.GetModule<InputModule>().SystemController.Disable();                //
            App.GetModule<InputModule>().DialogueController.Enable();               //
            App.CanvasManager.AddCanvas(_dialogueCanvas);
            App.CanvasManager.OpenCanvas(App.CanvasManager.GetCanvasContainer("Dialogue Canvas"), true);
            ConversationManager.Instance.StartConversation(_currentConversation);

            StartCoroutine(ProgressDialogue());
        }

        private IEnumerator ProgressDialogue()
        {
            yield return new WaitForSeconds(_fadeDelay);
            _closable = true;
            _dialogueFinished = false;

            while (ConversationManager.Instance.IsConversationActive)
            {
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
                ConversationManager.Instance.PressSelectedOption();
            }
        }

        private void Start()
        {
            App.GetModule<InputModule>().DialogueController.Dialogue.Cancel.performed += StopDialogue;
            App.GetModule<InputModule>().DialogueController.Dialogue.NextOption.performed += NextOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.PreviousOption.performed += PreviousOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Select.performed += SelectOption;
        }

        private void OnDestroy()
        {
            App.GetModule<InputModule>().DialogueController.Dialogue.Cancel.performed -= StopDialogue;
            App.GetModule<InputModule>().DialogueController.Dialogue.NextOption.performed -= NextOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.PreviousOption.performed -= PreviousOption;
            App.GetModule<InputModule>().DialogueController.Dialogue.Select.performed -= SelectOption;
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