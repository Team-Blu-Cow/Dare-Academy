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
            ""name"": ""Player"",
            ""id"": ""cea43b66-3806-4625-91fe-3e770bcc9539"",
            ""actions"": [
                {
                    ""name"": ""ExecuteStep"",
                    ""type"": ""Button"",
                    ""id"": ""5cd42ae5-fbb7-4695-abac-917154a20c35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AbilityMode"",
                    ""type"": ""Button"",
                    ""id"": ""5dab3b85-635d-4574-883c-1e1de15c3ed5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapAbilityR"",
                    ""type"": ""Button"",
                    ""id"": ""3766b5d7-4160-4d02-b7d0-f1f15bdd0bca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapAbilityL"",
                    ""type"": ""Button"",
                    ""id"": ""a46941af-80c4-4150-85c5-106cbbe91a83"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CancelAbility"",
                    ""type"": ""Button"",
                    ""id"": ""14452790-1c30-4091-be38-c4764af37671"",
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
                    ""name"": ""Direction"",
                    ""type"": ""Value"",
                    ""id"": ""6f4599d6-bec4-4457-b962-de2c297adc9d"",
                    ""expectedControlType"": ""Vector2"",
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
                    ""groups"": ""Keyboard"",
                    ""action"": ""ExecuteStep"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce0fbc98-3e8c-47c5-b40a-a7bcba1ff158"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ExecuteStep"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""872341c3-5118-4d9f-932e-0da7fa9538bc"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0abba543-e73b-474b-b5b2-8fb79873f89c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a9ac528b-a196-4d9e-a478-ce3e9b8d8a85"",
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
                    ""id"": ""7e7de46b-e5d6-4008-982c-3cfb9d4aba45"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""47370f12-271c-4853-8772-1f8c218959d9"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""45bfdd2e-345c-4df0-8c05-ebabfa8db8d6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""61a751ad-219a-4358-bd09-decf0de8168d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a02dd8fc-36b0-4ab2-b750-f51ade05b57a"",
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
                    ""id"": ""5ec9ea34-8837-49e0-bad9-2697acbe4ea6"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5b3892fb-2bb1-4511-bb4b-c618a5138043"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fef0d3ea-6f15-4a67-b582-4180456f995d"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7dcb537c-0684-4afb-88f5-3aa146bdcbd4"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Dpad"",
                    ""id"": ""7d9d2b21-069c-4bf0-825e-d174cfec73a9"",
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
                    ""id"": ""4eecc1ca-ee95-4436-9a59-8e9688b94e66"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1bbaffdd-62e3-47ad-809a-43232508b2a0"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""52eaaa07-8594-41ae-9ec2-1873b39d039c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8419d33b-2070-4251-8496-c8fe312faf4e"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6e71d388-db53-4313-b1a6-1af262fabda8"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""CancelAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5635d99-d30e-487a-b7f7-0679c0116eb3"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CancelAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""993a1a1c-db85-4663-9c7b-053de0dc72c4"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SwapAbilityL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d5a470a-243c-470b-992b-b9fa7f9691d7"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapAbilityL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2aa528a-ea6b-4383-ba24-39f24c01e35d"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SwapAbilityR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd488955-37c1-4861-b89b-92588f728d02"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwapAbilityR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8477489f-d56d-49cf-be88-76756f190465"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""AbilityMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d3582da-a4f2-4868-8dba-ea9c51b266d5"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""AbilityMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": []
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_ExecuteStep = m_Player.FindAction("ExecuteStep", throwIfNotFound: true);
        m_Player_AbilityMode = m_Player.FindAction("AbilityMode", throwIfNotFound: true);
        m_Player_SwapAbilityR = m_Player.FindAction("SwapAbilityR", throwIfNotFound: true);
        m_Player_SwapAbilityL = m_Player.FindAction("SwapAbilityL", throwIfNotFound: true);
        m_Player_CancelAbility = m_Player.FindAction("CancelAbility", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_Direction = m_Player.FindAction("Direction", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_ExecuteStep;
    private readonly InputAction m_Player_AbilityMode;
    private readonly InputAction m_Player_SwapAbilityR;
    private readonly InputAction m_Player_SwapAbilityL;
    private readonly InputAction m_Player_CancelAbility;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_Direction;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ExecuteStep => m_Wrapper.m_Player_ExecuteStep;
        public InputAction @AbilityMode => m_Wrapper.m_Player_AbilityMode;
        public InputAction @SwapAbilityR => m_Wrapper.m_Player_SwapAbilityR;
        public InputAction @SwapAbilityL => m_Wrapper.m_Player_SwapAbilityL;
        public InputAction @CancelAbility => m_Wrapper.m_Player_CancelAbility;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @Direction => m_Wrapper.m_Player_Direction;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @ExecuteStep.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExecuteStep;
                @ExecuteStep.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExecuteStep;
                @ExecuteStep.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnExecuteStep;
                @AbilityMode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilityMode;
                @AbilityMode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilityMode;
                @AbilityMode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilityMode;
                @SwapAbilityR.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityR.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityR.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityR;
                @SwapAbilityL.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityL;
                @SwapAbilityL.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityL;
                @SwapAbilityL.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwapAbilityL;
                @CancelAbility.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancelAbility;
                @CancelAbility.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancelAbility;
                @CancelAbility.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancelAbility;
                @Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Direction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDirection;
                @Direction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDirection;
                @Direction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDirection;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ExecuteStep.started += instance.OnExecuteStep;
                @ExecuteStep.performed += instance.OnExecuteStep;
                @ExecuteStep.canceled += instance.OnExecuteStep;
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
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Direction.started += instance.OnDirection;
                @Direction.performed += instance.OnDirection;
                @Direction.canceled += instance.OnDirection;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnExecuteStep(InputAction.CallbackContext context);
        void OnAbilityMode(InputAction.CallbackContext context);
        void OnSwapAbilityR(InputAction.CallbackContext context);
        void OnSwapAbilityL(InputAction.CallbackContext context);
        void OnCancelAbility(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDirection(InputAction.CallbackContext context);
    }
}
