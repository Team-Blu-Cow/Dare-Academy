// GENERATED AUTOMATICALLY FROM 'Assets/Res/Input Schemes/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""MoveKeyboard"",
            ""id"": ""cea43b66-3806-4625-91fe-3e770bcc9539"",
            ""actions"": [
                {
                    ""name"": ""Step"",
                    ""type"": ""Button"",
                    ""id"": ""5cd42ae5-fbb7-4695-abac-917154a20c35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0c6e4a77-4f0c-4767-8a0a-1bdc82e20b95"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveNorth"",
                    ""type"": ""Button"",
                    ""id"": ""e4a083d8-191b-4f2c-b043-20862368a693"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveEast"",
                    ""type"": ""Button"",
                    ""id"": ""f4f9ff0d-bd6c-4048-8fa0-bbb84696ea1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveSouth"",
                    ""type"": ""Button"",
                    ""id"": ""6a230f9b-1d52-403c-a027-37e93a523879"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveWest"",
                    ""type"": ""Button"",
                    ""id"": ""ee535085-e52b-4419-b842-1d3d15abb7ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d5fbfa61-d695-447f-9936-150613164d6d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Step"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""872341c3-5118-4d9f-932e-0da7fa9538bc"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14a8b2e5-6672-4901-8ecf-37ff7004e381"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveNorth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bac0180a-8948-4ca1-80ec-7c7823a43e40"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveEast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a07e6f7c-d651-4c2b-b709-cf0a646cf8a0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveSouth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9abf1bfa-8963-42c8-aa90-8eae3622a053"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveWest"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MoveGamepad"",
            ""id"": ""a0f42282-78ed-4814-b18e-ab80b99c5b9a"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""15ab76bc-5241-40cd-924b-5076736e00de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""68c2c214-b857-492f-aec6-b09544449c73"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Ability"",
            ""id"": ""31809891-72d2-4334-bddc-d87af4db1231"",
            ""actions"": [
                {
                    ""name"": ""AbilityMode"",
                    ""type"": ""Button"",
                    ""id"": ""5314b8c6-3e77-4b1b-9a9c-7f87eafb7a68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapAbilityR"",
                    ""type"": ""Button"",
                    ""id"": ""6016c215-0a11-43f1-975b-23d3c177c549"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapAbilityL"",
                    ""type"": ""Button"",
                    ""id"": ""84fdb7d5-0c13-4a3e-9778-08969f504a55"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CancelAbility"",
                    ""type"": ""Button"",
                    ""id"": ""a234393d-6b6e-42c0-b05e-20a777f96004"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""caf03df1-e514-4573-b13c-457478f73762"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""AbilityMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""815c7b70-8379-492f-92e3-84b243d1dcc3"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""SwapAbilityR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e82c512-2e7e-4045-b1fc-c6a5b981e411"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""SwapAbilityL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f10b2e7a-2dd9-4760-b527-d723c165fa07"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CancelAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KB&Mouse"",
            ""bindingGroup"": ""KB&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""gamepad"",
            ""bindingGroup"": ""gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // MoveKeyboard
        m_MoveKeyboard = asset.FindActionMap("MoveKeyboard", throwIfNotFound: true);
        m_MoveKeyboard_Step = m_MoveKeyboard.FindAction("Step", throwIfNotFound: true);
        m_MoveKeyboard_Interact = m_MoveKeyboard.FindAction("Interact", throwIfNotFound: true);
        m_MoveKeyboard_MoveNorth = m_MoveKeyboard.FindAction("MoveNorth", throwIfNotFound: true);
        m_MoveKeyboard_MoveEast = m_MoveKeyboard.FindAction("MoveEast", throwIfNotFound: true);
        m_MoveKeyboard_MoveSouth = m_MoveKeyboard.FindAction("MoveSouth", throwIfNotFound: true);
        m_MoveKeyboard_MoveWest = m_MoveKeyboard.FindAction("MoveWest", throwIfNotFound: true);
        // MoveGamepad
        m_MoveGamepad = asset.FindActionMap("MoveGamepad", throwIfNotFound: true);
        m_MoveGamepad_Newaction = m_MoveGamepad.FindAction("New action", throwIfNotFound: true);
        // Ability
        m_Ability = asset.FindActionMap("Ability", throwIfNotFound: true);
        m_Ability_AbilityMode = m_Ability.FindAction("AbilityMode", throwIfNotFound: true);
        m_Ability_SwapAbilityR = m_Ability.FindAction("SwapAbilityR", throwIfNotFound: true);
        m_Ability_SwapAbilityL = m_Ability.FindAction("SwapAbilityL", throwIfNotFound: true);
        m_Ability_CancelAbility = m_Ability.FindAction("CancelAbility", throwIfNotFound: true);
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

    // MoveKeyboard
    private readonly InputActionMap m_MoveKeyboard;
    private IMoveKeyboardActions m_MoveKeyboardActionsCallbackInterface;
    private readonly InputAction m_MoveKeyboard_Step;
    private readonly InputAction m_MoveKeyboard_Interact;
    private readonly InputAction m_MoveKeyboard_MoveNorth;
    private readonly InputAction m_MoveKeyboard_MoveEast;
    private readonly InputAction m_MoveKeyboard_MoveSouth;
    private readonly InputAction m_MoveKeyboard_MoveWest;
    public struct MoveKeyboardActions
    {
        private @PlayerControls m_Wrapper;
        public MoveKeyboardActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Step => m_Wrapper.m_MoveKeyboard_Step;
        public InputAction @Interact => m_Wrapper.m_MoveKeyboard_Interact;
        public InputAction @MoveNorth => m_Wrapper.m_MoveKeyboard_MoveNorth;
        public InputAction @MoveEast => m_Wrapper.m_MoveKeyboard_MoveEast;
        public InputAction @MoveSouth => m_Wrapper.m_MoveKeyboard_MoveSouth;
        public InputAction @MoveWest => m_Wrapper.m_MoveKeyboard_MoveWest;
        public InputActionMap Get() { return m_Wrapper.m_MoveKeyboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MoveKeyboardActions set) { return set.Get(); }
        public void SetCallbacks(IMoveKeyboardActions instance)
        {
            if (m_Wrapper.m_MoveKeyboardActionsCallbackInterface != null)
            {
                @Step.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnStep;
                @Step.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnStep;
                @Step.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnStep;
                @Interact.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnInteract;
                @MoveNorth.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveNorth;
                @MoveNorth.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveNorth;
                @MoveNorth.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveNorth;
                @MoveEast.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveEast;
                @MoveEast.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveEast;
                @MoveEast.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveEast;
                @MoveSouth.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveSouth;
                @MoveSouth.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveSouth;
                @MoveSouth.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveSouth;
                @MoveWest.started -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveWest;
                @MoveWest.performed -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveWest;
                @MoveWest.canceled -= m_Wrapper.m_MoveKeyboardActionsCallbackInterface.OnMoveWest;
            }
            m_Wrapper.m_MoveKeyboardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Step.started += instance.OnStep;
                @Step.performed += instance.OnStep;
                @Step.canceled += instance.OnStep;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @MoveNorth.started += instance.OnMoveNorth;
                @MoveNorth.performed += instance.OnMoveNorth;
                @MoveNorth.canceled += instance.OnMoveNorth;
                @MoveEast.started += instance.OnMoveEast;
                @MoveEast.performed += instance.OnMoveEast;
                @MoveEast.canceled += instance.OnMoveEast;
                @MoveSouth.started += instance.OnMoveSouth;
                @MoveSouth.performed += instance.OnMoveSouth;
                @MoveSouth.canceled += instance.OnMoveSouth;
                @MoveWest.started += instance.OnMoveWest;
                @MoveWest.performed += instance.OnMoveWest;
                @MoveWest.canceled += instance.OnMoveWest;
            }
        }
    }
    public MoveKeyboardActions @MoveKeyboard => new MoveKeyboardActions(this);

    // MoveGamepad
    private readonly InputActionMap m_MoveGamepad;
    private IMoveGamepadActions m_MoveGamepadActionsCallbackInterface;
    private readonly InputAction m_MoveGamepad_Newaction;
    public struct MoveGamepadActions
    {
        private @PlayerControls m_Wrapper;
        public MoveGamepadActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_MoveGamepad_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_MoveGamepad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MoveGamepadActions set) { return set.Get(); }
        public void SetCallbacks(IMoveGamepadActions instance)
        {
            if (m_Wrapper.m_MoveGamepadActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_MoveGamepadActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_MoveGamepadActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_MoveGamepadActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_MoveGamepadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public MoveGamepadActions @MoveGamepad => new MoveGamepadActions(this);

    // Ability
    private readonly InputActionMap m_Ability;
    private IAbilityActions m_AbilityActionsCallbackInterface;
    private readonly InputAction m_Ability_AbilityMode;
    private readonly InputAction m_Ability_SwapAbilityR;
    private readonly InputAction m_Ability_SwapAbilityL;
    private readonly InputAction m_Ability_CancelAbility;
    public struct AbilityActions
    {
        private @PlayerControls m_Wrapper;
        public AbilityActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @AbilityMode => m_Wrapper.m_Ability_AbilityMode;
        public InputAction @SwapAbilityR => m_Wrapper.m_Ability_SwapAbilityR;
        public InputAction @SwapAbilityL => m_Wrapper.m_Ability_SwapAbilityL;
        public InputAction @CancelAbility => m_Wrapper.m_Ability_CancelAbility;
        public InputActionMap Get() { return m_Wrapper.m_Ability; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AbilityActions set) { return set.Get(); }
        public void SetCallbacks(IAbilityActions instance)
        {
            if (m_Wrapper.m_AbilityActionsCallbackInterface != null)
            {
                @AbilityMode.started -= m_Wrapper.m_AbilityActionsCallbackInterface.OnAbilityMode;
                @AbilityMode.performed -= m_Wrapper.m_AbilityActionsCallbackInterface.OnAbilityMode;
                @AbilityMode.canceled -= m_Wrapper.m_AbilityActionsCallbackInterface.OnAbilityMode;
                @SwapAbilityR.started -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityR.performed -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityR.canceled -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityL.started -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityL;
                @SwapAbilityL.performed -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityL;
                @SwapAbilityL.canceled -= m_Wrapper.m_AbilityActionsCallbackInterface.OnSwapAbilityL;
                @CancelAbility.started -= m_Wrapper.m_AbilityActionsCallbackInterface.OnCancelAbility;
                @CancelAbility.performed -= m_Wrapper.m_AbilityActionsCallbackInterface.OnCancelAbility;
                @CancelAbility.canceled -= m_Wrapper.m_AbilityActionsCallbackInterface.OnCancelAbility;
            }
            m_Wrapper.m_AbilityActionsCallbackInterface = instance;
            if (instance != null)
            {
                @AbilityMode.started += instance.OnAbilityMode;
                @AbilityMode.performed += instance.OnAbilityMode;
                @AbilityMode.canceled += instance.OnAbilityMode;
                @SwapAbilityR.started += instance.OnSwapAbilityR;
                @SwapAbilityR.performed += instance.OnSwapAbilityR;
                @SwapAbilityR.canceled += instance.OnSwapAbilityR;
                @SwapAbilityL.started += instance.OnSwapAbilityL;
                @SwapAbilityL.performed += instance.OnSwapAbilityL;
                @SwapAbilityL.canceled += instance.OnSwapAbilityL;
                @CancelAbility.started += instance.OnCancelAbility;
                @CancelAbility.performed += instance.OnCancelAbility;
                @CancelAbility.canceled += instance.OnCancelAbility;
            }
        }
    }
    public AbilityActions @Ability => new AbilityActions(this);
    private int m_KBMouseSchemeIndex = -1;
    public InputControlScheme KBMouseScheme
    {
        get
        {
            if (m_KBMouseSchemeIndex == -1) m_KBMouseSchemeIndex = asset.FindControlSchemeIndex("KB&Mouse");
            return asset.controlSchemes[m_KBMouseSchemeIndex];
        }
    }
    private int m_gamepadSchemeIndex = -1;
    public InputControlScheme gamepadScheme
    {
        get
        {
            if (m_gamepadSchemeIndex == -1) m_gamepadSchemeIndex = asset.FindControlSchemeIndex("gamepad");
            return asset.controlSchemes[m_gamepadSchemeIndex];
        }
    }
    public interface IMoveKeyboardActions
    {
        void OnStep(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnMoveNorth(InputAction.CallbackContext context);
        void OnMoveEast(InputAction.CallbackContext context);
        void OnMoveSouth(InputAction.CallbackContext context);
        void OnMoveWest(InputAction.CallbackContext context);
    }
    public interface IMoveGamepadActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IAbilityActions
    {
        void OnAbilityMode(InputAction.CallbackContext context);
        void OnSwapAbilityR(InputAction.CallbackContext context);
        void OnSwapAbilityL(InputAction.CallbackContext context);
        void OnCancelAbility(InputAction.CallbackContext context);
    }
}
