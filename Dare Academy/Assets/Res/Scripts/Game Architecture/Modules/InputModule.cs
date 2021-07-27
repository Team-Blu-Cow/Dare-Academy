using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

namespace blu
{
    public class InputModule : Module
    {
        private SystemContols _systemControls;
        public SystemContols SystemController { get => _systemControls; }

        private PlayerControls _playerControls;
        public PlayerControls PlayerController { get => _playerControls; }

        private DialogueControls _dialogueControls;
        public DialogueControls DialogueController { get => _dialogueControls; }

        private InputDevice m_LastUsedDevice;
        public InputDevice LastUsedDevice { get => m_LastUsedDevice; }

        public delegate void LastDeviceChangedDelegate();

        public event LastDeviceChangedDelegate LastDeviceChanged;

        public override void Initialize()
        {
            // If functions run twice after scene switch take a look at this: https://answers.unity.com/questions/1767382/removing-event-from-new-input-system-not-working.html
            Debug.Log("[App]: Initializing input module");
            SetUpControllers();
        }

        private void SetUpControllers()
        {
            _systemControls = new SystemContols();
            _playerControls = new PlayerControls();
            _dialogueControls = new DialogueControls();
            _systemControls.Enable();
            _playerControls.Enable();
        }

        private void OnEnable()
        {
            InputSystem.onEvent += OnInputDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onEvent -= OnInputDeviceChange;
        }

        private void OnInputDeviceChange(InputEventPtr eventPtr, InputDevice device)
        {
            if (m_LastUsedDevice == device)
                return;

            m_LastUsedDevice = device;
            LastDeviceChanged?.Invoke();
        }
    }
}