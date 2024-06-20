//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Prefabs/NeedForLevelToWork/KeyBindings.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @KeyBindings: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @KeyBindings()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""KeyBindings"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""656b6b51-4522-4cd8-b915-b2de3d28b72e"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""27a21338-d893-4300-8d40-1c7c04af3a46"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""681eab27-6cf6-4504-9af5-5c2b5c3fae02"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Melee"",
                    ""type"": ""Button"",
                    ""id"": ""52271d57-6fe8-4307-b4fc-4172e0839931"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""e3fe39eb-aa1a-44ab-9612-7d3efb601beb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestShit"",
                    ""type"": ""Button"",
                    ""id"": ""fcc5745b-6040-4096-b3a4-c1bae575eb68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""83679b6b-4a51-4c1a-a280-7413ccb131d7"",
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
                    ""id"": ""1db73699-cfc1-42c5-add4-47293244ce5e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e5c1001-bf94-4674-965d-78e0776a8b3e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0fb0ac92-bd72-450f-98c3-d8579e30218a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d975f599-1ce4-446d-84e4-55b6fb43a6e4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""5161e669-e98a-44bd-86fa-024a051e890d"",
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
                    ""id"": ""41f63297-1662-47f3-89c4-0a788e88aea7"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c17b1225-d5b5-46a6-a4a8-501f3aafed0b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""23e63b83-5186-4bb4-b1c7-c7497e1be443"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""cbefd7f9-7e7e-43d7-b2b4-2439ca9f91fc"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b67e8782-c7dc-42df-a8b0-02dfd772eafe"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6be337de-0797-4e99-97ca-18ad7bf35444"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Melee"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c245f6e3-5ab9-417b-a5d2-82e952ae81e0"",
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
                    ""id"": ""0d141ae9-4169-47ec-a6f9-46de126629ff"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestShit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PauseUI"",
            ""id"": ""1cb7c8f1-ad52-45e2-8bd5-ba7c58078bde"",
            ""actions"": [
                {
                    ""name"": ""Resume"",
                    ""type"": ""Button"",
                    ""id"": ""e675d9fe-d9fc-4f27-8b63-a708a1c09902"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""74ab5d8e-7108-4993-a94c-c8bc1b2f045a"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Resume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""RebindingKeepEmpty"",
            ""id"": ""3c80cebd-6beb-422d-a657-9721c57d9358"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""ad23ff11-896f-4f97-b882-d93b755d9505"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f3a1adb7-9c1f-4329-98a1-e6249dd21223"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Dash = m_Gameplay.FindAction("Dash", throwIfNotFound: true);
        m_Gameplay_Melee = m_Gameplay.FindAction("Melee", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
        m_Gameplay_TestShit = m_Gameplay.FindAction("TestShit", throwIfNotFound: true);
        // PauseUI
        m_PauseUI = asset.FindActionMap("PauseUI", throwIfNotFound: true);
        m_PauseUI_Resume = m_PauseUI.FindAction("Resume", throwIfNotFound: true);
        // RebindingKeepEmpty
        m_RebindingKeepEmpty = asset.FindActionMap("RebindingKeepEmpty", throwIfNotFound: true);
        m_RebindingKeepEmpty_Newaction = m_RebindingKeepEmpty.FindAction("New action", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Dash;
    private readonly InputAction m_Gameplay_Melee;
    private readonly InputAction m_Gameplay_Pause;
    private readonly InputAction m_Gameplay_TestShit;
    public struct GameplayActions
    {
        private @KeyBindings m_Wrapper;
        public GameplayActions(@KeyBindings wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Dash => m_Wrapper.m_Gameplay_Dash;
        public InputAction @Melee => m_Wrapper.m_Gameplay_Melee;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputAction @TestShit => m_Wrapper.m_Gameplay_TestShit;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Melee.started += instance.OnMelee;
            @Melee.performed += instance.OnMelee;
            @Melee.canceled += instance.OnMelee;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @TestShit.started += instance.OnTestShit;
            @TestShit.performed += instance.OnTestShit;
            @TestShit.canceled += instance.OnTestShit;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Melee.started -= instance.OnMelee;
            @Melee.performed -= instance.OnMelee;
            @Melee.canceled -= instance.OnMelee;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @TestShit.started -= instance.OnTestShit;
            @TestShit.performed -= instance.OnTestShit;
            @TestShit.canceled -= instance.OnTestShit;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // PauseUI
    private readonly InputActionMap m_PauseUI;
    private List<IPauseUIActions> m_PauseUIActionsCallbackInterfaces = new List<IPauseUIActions>();
    private readonly InputAction m_PauseUI_Resume;
    public struct PauseUIActions
    {
        private @KeyBindings m_Wrapper;
        public PauseUIActions(@KeyBindings wrapper) { m_Wrapper = wrapper; }
        public InputAction @Resume => m_Wrapper.m_PauseUI_Resume;
        public InputActionMap Get() { return m_Wrapper.m_PauseUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseUIActions set) { return set.Get(); }
        public void AddCallbacks(IPauseUIActions instance)
        {
            if (instance == null || m_Wrapper.m_PauseUIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PauseUIActionsCallbackInterfaces.Add(instance);
            @Resume.started += instance.OnResume;
            @Resume.performed += instance.OnResume;
            @Resume.canceled += instance.OnResume;
        }

        private void UnregisterCallbacks(IPauseUIActions instance)
        {
            @Resume.started -= instance.OnResume;
            @Resume.performed -= instance.OnResume;
            @Resume.canceled -= instance.OnResume;
        }

        public void RemoveCallbacks(IPauseUIActions instance)
        {
            if (m_Wrapper.m_PauseUIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPauseUIActions instance)
        {
            foreach (var item in m_Wrapper.m_PauseUIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PauseUIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PauseUIActions @PauseUI => new PauseUIActions(this);

    // RebindingKeepEmpty
    private readonly InputActionMap m_RebindingKeepEmpty;
    private List<IRebindingKeepEmptyActions> m_RebindingKeepEmptyActionsCallbackInterfaces = new List<IRebindingKeepEmptyActions>();
    private readonly InputAction m_RebindingKeepEmpty_Newaction;
    public struct RebindingKeepEmptyActions
    {
        private @KeyBindings m_Wrapper;
        public RebindingKeepEmptyActions(@KeyBindings wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_RebindingKeepEmpty_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_RebindingKeepEmpty; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RebindingKeepEmptyActions set) { return set.Get(); }
        public void AddCallbacks(IRebindingKeepEmptyActions instance)
        {
            if (instance == null || m_Wrapper.m_RebindingKeepEmptyActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_RebindingKeepEmptyActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IRebindingKeepEmptyActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IRebindingKeepEmptyActions instance)
        {
            if (m_Wrapper.m_RebindingKeepEmptyActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IRebindingKeepEmptyActions instance)
        {
            foreach (var item in m_Wrapper.m_RebindingKeepEmptyActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_RebindingKeepEmptyActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public RebindingKeepEmptyActions @RebindingKeepEmpty => new RebindingKeepEmptyActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnMelee(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnTestShit(InputAction.CallbackContext context);
    }
    public interface IPauseUIActions
    {
        void OnResume(InputAction.CallbackContext context);
    }
    public interface IRebindingKeepEmptyActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
