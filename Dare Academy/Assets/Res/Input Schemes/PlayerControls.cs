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
            ""name"": ""Move"",
            ""id"": ""cea43b66-3806-4625-91fe-3e770bcc9539"",
            ""actions"": [
                {
                    ""name"": ""Direction"",
                    ""type"": ""Button"",
                    ""id"": ""a800f43b-f2c9-46b7-9104-eae6bc27e55b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
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
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard WASD"",
                    ""id"": ""007e638e-b312-408b-9428-9448999df98a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""063127de-b3ed-47d6-915d-342054dafc05"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""435b69b6-5a3f-4d99-ae49-6f7c60988139"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1b4d1a42-2d8e-4180-8ed4-d9782db30a9f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""503df545-4074-483d-8efb-1651c8586025"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad D-Pad"",
                    ""id"": ""01963605-a191-45bd-81d7-0309ddbc88f9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""102707af-b04f-4d9f-9fc8-31dc434fd103"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c830b48c-e9ba-4893-a5d0-6126b417dfdc"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d74bfb00-f7bc-4122-a037-b5ad8c1a311c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0327109c-29a5-4718-856e-08d901324e2b"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad Left Stick"",
                    ""id"": ""dc2216e5-5a16-4d48-8a2c-3bdf3d17f686"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e8acffae-afbc-44f1-8f2a-0bb6ccf71eb2"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2454ce30-2a53-4252-8962-a7cd84db6353"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""79c7c842-8854-4367-a71b-19630c7b91b2"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b6c2c65e-5b08-4d97-a7fe-a22c9383d2ce"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
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
        // Move
        m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
        m_Move_Direction = m_Move.FindAction("Direction", throwIfNotFound: true);
        m_Move_Step = m_Move.FindAction("Step", throwIfNotFound: true);
        m_Move_Interact = m_Move.FindAction("Interact", throwIfNotFound: true);
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

    // Move
    private readonly InputActionMap m_Move;
    private IMoveActions m_MoveActionsCallbackInterface;
    private readonly InputAction m_Move_Direction;
    private readonly InputAction m_Move_Step;
    private readonly InputAction m_Move_Interact;
    public struct MoveActions
    {
        private @PlayerControls m_Wrapper;
        public MoveActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Direction => m_Wrapper.m_Move_Direction;
        public InputAction @Step => m_Wrapper.m_Move_Step;
        public InputAction @Interact => m_Wrapper.m_Move_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Move; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MoveActions set) { return set.Get(); }
        public void SetCallbacks(IMoveActions instance)
        {
            if (m_Wrapper.m_MoveActionsCallbackInterface != null)
            {
                @Direction.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnDirection;
                @Direction.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnDirection;
                @Direction.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnDirection;
                @Step.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnStep;
                @Step.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnStep;
                @Step.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnStep;
                @Interact.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_MoveActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Direction.started += instance.OnDirection;
                @Direction.performed += instance.OnDirection;
                @Direction.canceled += instance.OnDirection;
                @Step.started += instance.OnStep;
                @Step.performed += instance.OnStep;
                @Step.canceled += instance.OnStep;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public MoveActions @Move => new MoveActions(this);

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
    public interface IMoveActions
    {
        void OnDirection(InputAction.CallbackContext context);
        void OnStep(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface IAbilityActions
    {
        void OnAbilityMode(InputAction.CallbackContext context);
        void OnSwapAbilityR(InputAction.CallbackContext context);
        void OnSwapAbilityL(InputAction.CallbackContext context);
        void OnCancelAbility(InputAction.CallbackContext context);
    }
}
