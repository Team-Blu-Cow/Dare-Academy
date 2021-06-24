// GENERATED AUTOMATICALLY FROM 'Assets/Res/Input Schemes/DialogueControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DialogueControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DialogueControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DialogueControls"",
    ""maps"": [
        {
            ""name"": ""Dialogue"",
            ""id"": ""38e7fa75-8b8d-4e0f-91b2-cadd4e974244"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""74d60d6b-6132-48a0-897a-b6d744084994"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""a5077c45-72ba-4302-b8e3-c63dc049d793"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next Option"",
                    ""type"": ""Button"",
                    ""id"": ""2ff749a7-f2ba-4363-9f1f-ad4f5fdb4632"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Previous Option"",
                    ""type"": ""Button"",
                    ""id"": ""fe668aec-e351-49cc-8178-abaf99596c65"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c438f781-dd41-461c-9765-ce8673a97fd6"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf956f2f-4700-43be-9191-5859b84b65d4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""881599ab-8ad6-41b0-b5d4-addba79c0caa"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06f7e67c-76f5-4cee-945d-5a6985f2bf97"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34f86036-8578-4f4a-ade4-9270cdc5acdf"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc0626d4-226e-48bf-9df0-6bca71c30a23"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e2b3631-9406-4e70-85d0-7d8a654b5d19"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1478003f-8011-436f-b66f-78b768187ad7"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a445e59-f96d-4352-b2e9-a32a053b6eb0"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""263d84e4-70fb-403e-bb36-7c158e58c2d2"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d7b24cc-e902-4d2b-9389-a55ec3b95b6e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e5634aa-e422-4ff8-af6b-3f5dcd2996f6"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e444bc35-3f9f-443a-9acd-42fccaa4dfed"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cf68fd5-189e-44a1-af0a-1cbb41e68ba8"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""975da34d-e9a4-4390-ab41-2c32998c2493"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Option"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Dialogue
        m_Dialogue = asset.FindActionMap("Dialogue", throwIfNotFound: true);
        m_Dialogue_Select = m_Dialogue.FindAction("Select", throwIfNotFound: true);
        m_Dialogue_Cancel = m_Dialogue.FindAction("Cancel", throwIfNotFound: true);
        m_Dialogue_NextOption = m_Dialogue.FindAction("Next Option", throwIfNotFound: true);
        m_Dialogue_PreviousOption = m_Dialogue.FindAction("Previous Option", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Dialogue
    private readonly InputActionMap m_Dialogue;
    private IDialogueActions m_DialogueActionsCallbackInterface;
    private readonly InputAction m_Dialogue_Select;
    private readonly InputAction m_Dialogue_Cancel;
    private readonly InputAction m_Dialogue_NextOption;
    private readonly InputAction m_Dialogue_PreviousOption;
    public struct DialogueActions
    {
        private @DialogueControls m_Wrapper;
        public DialogueActions(@DialogueControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_Dialogue_Select;
        public InputAction @Cancel => m_Wrapper.m_Dialogue_Cancel;
        public InputAction @NextOption => m_Wrapper.m_Dialogue_NextOption;
        public InputAction @PreviousOption => m_Wrapper.m_Dialogue_PreviousOption;
        public InputActionMap Get() { return m_Wrapper.m_Dialogue; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DialogueActions set) { return set.Get(); }
        public void SetCallbacks(IDialogueActions instance)
        {
            if (m_Wrapper.m_DialogueActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnSelect;
                @Cancel.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCancel;
                @NextOption.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnNextOption;
                @NextOption.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnNextOption;
                @NextOption.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnNextOption;
                @PreviousOption.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnPreviousOption;
                @PreviousOption.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnPreviousOption;
                @PreviousOption.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnPreviousOption;
            }
            m_Wrapper.m_DialogueActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @NextOption.started += instance.OnNextOption;
                @NextOption.performed += instance.OnNextOption;
                @NextOption.canceled += instance.OnNextOption;
                @PreviousOption.started += instance.OnPreviousOption;
                @PreviousOption.performed += instance.OnPreviousOption;
                @PreviousOption.canceled += instance.OnPreviousOption;
            }
        }
    }
    public DialogueActions @Dialogue => new DialogueActions(this);
    public interface IDialogueActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnNextOption(InputAction.CallbackContext context);
        void OnPreviousOption(InputAction.CallbackContext context);
    }
}
