// GENERATED AUTOMATICALLY FROM 'Assets/Res/Input Schemes/SystemContols.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @SystemContols : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @SystemContols()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SystemContols"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""ec260056-511f-41df-abb2-17eb82335826"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""545bb07b-e7a8-409e-8438-4bac5a31abff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f828fbcd-afde-4f3e-a714-f5fcafa20272"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ec949e3-7e9a-4ac9-ae9b-1cac6beee9ec"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MapControlls"",
            ""id"": ""f4c387fb-fdf5-4df0-86a2-353ed1936dd0"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""2ba8f0ca-9c0f-42e0-876b-18dd277a910e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Button"",
                    ""id"": ""062d0344-b9e7-479f-a644-ccae26d0bf53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomIn"",
                    ""type"": ""Button"",
                    ""id"": ""2d62562e-7127-4936-a4e3-c5e4e4335a29"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ZoomOut"",
                    ""type"": ""Button"",
                    ""id"": ""329e84eb-df0a-48bf-a638-8453a3739d74"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard WASD"",
                    ""id"": ""91add1cb-3ef9-4f68-a95f-1e2094b2753a"",
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
                    ""id"": ""f8340c87-fcc8-46a4-939f-73747b10d223"",
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
                    ""id"": ""3f176358-da45-4713-8422-fdbbf53f80c1"",
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
                    ""id"": ""b46b891e-fa5e-44db-8ce7-0afd3c6c23fb"",
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
                    ""id"": ""f9d82075-b23e-42bb-bfc3-bc0b2a753e36"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard WASD"",
                    ""id"": ""35e595e3-2167-4742-81db-1b76c68aee7c"",
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
                    ""id"": ""bddd447b-2d5d-4efd-b950-8f5f923848d3"",
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
                    ""id"": ""6efd8ecb-6946-4332-a5ec-5d07b009ae65"",
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
                    ""id"": ""1610ad52-4fd9-4ada-b8d4-2d96a8f1e6f0"",
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
                    ""id"": ""bc3af054-88a8-4557-88b4-3fad82540ae8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard WASD"",
                    ""id"": ""4de7ca00-b938-4b31-9c7f-6684e6a6b5d4"",
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
                    ""id"": ""dc0f1c08-bde0-4176-a2ae-66321dd7fd41"",
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
                    ""id"": ""6e2575b2-cd68-456c-82e0-de3d91562c33"",
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
                    ""id"": ""d6f6cad2-2a08-427f-8106-1e4ebdef4fa6"",
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
                    ""id"": ""441e5029-39ff-4da3-ab9c-c9505a1260c9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2763e8ce-a817-4977-a60b-bf7bc2964e82"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard WASD"",
                    ""id"": ""96097305-8fe2-41b8-834e-5a9eb67bf2ac"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""810a3689-5dfe-41d9-8267-f9658e188e58"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""46df4acd-ac61-4520-b14e-7db109db8491"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""55334bec-a54a-4d96-82a1-5c5b8027a6c4"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""dc3cd7c2-b3e9-4bbf-b012-907592853c49"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad Left Stick"",
                    ""id"": ""bee2ff16-3a2d-4fd7-86b3-38c5cdb25d5d"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""414fa14b-2444-4475-8456-772ca415a852"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""52aa58e9-7cd9-423f-a6ce-0f9f7fe7ffd6"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a48330e3-29f7-4f1b-a904-84487354576c"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""137783d9-11db-4430-92b5-e0752eb513f1"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad D-Pad"",
                    ""id"": ""19272d86-ed6b-4714-a25b-8539a4f01d08"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1b6aab40-6df7-4fbc-b888-19f4f409339f"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ad070b66-6868-4e01-99f3-899a22141690"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""22bddfc9-6ef9-405e-8f1e-4e33b8c43d7c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""736065bd-eb89-4384-874a-c81e25f2b2ee"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c26eed3d-f399-459d-a97e-fa62a556e2cb"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6710938-80d5-47cb-9f5c-3e6ed3d70f6b"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Pause = m_UI.FindAction("Pause", throwIfNotFound: true);
        // MapControlls
        m_MapControlls = asset.FindActionMap("MapControlls", throwIfNotFound: true);
        m_MapControlls_Move = m_MapControlls.FindAction("Move", throwIfNotFound: true);
        m_MapControlls_Mouse = m_MapControlls.FindAction("Mouse", throwIfNotFound: true);
        m_MapControlls_ZoomIn = m_MapControlls.FindAction("ZoomIn", throwIfNotFound: true);
        m_MapControlls_ZoomOut = m_MapControlls.FindAction("ZoomOut", throwIfNotFound: true);
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

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Pause;
    public struct UIActions
    {
        private @SystemContols m_Wrapper;
        public UIActions(@SystemContols wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_UI_Pause;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // MapControlls
    private readonly InputActionMap m_MapControlls;
    private IMapControllsActions m_MapControllsActionsCallbackInterface;
    private readonly InputAction m_MapControlls_Move;
    private readonly InputAction m_MapControlls_Mouse;
    private readonly InputAction m_MapControlls_ZoomIn;
    private readonly InputAction m_MapControlls_ZoomOut;
    public struct MapControllsActions
    {
        private @SystemContols m_Wrapper;
        public MapControllsActions(@SystemContols wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MapControlls_Move;
        public InputAction @Mouse => m_Wrapper.m_MapControlls_Mouse;
        public InputAction @ZoomIn => m_Wrapper.m_MapControlls_ZoomIn;
        public InputAction @ZoomOut => m_Wrapper.m_MapControlls_ZoomOut;
        public InputActionMap Get() { return m_Wrapper.m_MapControlls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapControllsActions set) { return set.Get(); }
        public void SetCallbacks(IMapControllsActions instance)
        {
            if (m_Wrapper.m_MapControllsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMove;
                @Mouse.started -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnMouse;
                @ZoomIn.started -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomIn;
                @ZoomIn.performed -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomIn;
                @ZoomIn.canceled -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomIn;
                @ZoomOut.started -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomOut;
                @ZoomOut.performed -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomOut;
                @ZoomOut.canceled -= m_Wrapper.m_MapControllsActionsCallbackInterface.OnZoomOut;
            }
            m_Wrapper.m_MapControllsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @ZoomIn.started += instance.OnZoomIn;
                @ZoomIn.performed += instance.OnZoomIn;
                @ZoomIn.canceled += instance.OnZoomIn;
                @ZoomOut.started += instance.OnZoomOut;
                @ZoomOut.performed += instance.OnZoomOut;
                @ZoomOut.canceled += instance.OnZoomOut;
            }
        }
    }
    public MapControllsActions @MapControlls => new MapControllsActions(this);
    public interface IUIActions
    {
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IMapControllsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnZoomIn(InputAction.CallbackContext context);
        void OnZoomOut(InputAction.CallbackContext context);
    }
}
