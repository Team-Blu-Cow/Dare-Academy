using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class PlayerInput
{
    public enum ControlMode
    {
        None,
        Gamepad,
        Keyboard,
        MultiKeyboardPriority,
        MultiGamepadPriority
    }

    private ControlMode m_currentMode = ControlMode.None;
    private PlayerControls m_input;
    private KeyboardInput m_keyboardInput;
    private GamepadInput m_gamepadInput;

    public void Init()
    {
        m_input = App.GetModule<InputModule>().PlayerController;
        m_keyboardInput = new KeyboardInput(m_input);
        m_keyboardInput.Init();

        m_gamepadInput = new GamepadInput(m_input);
        m_gamepadInput.Init();
    }

    public void Cleanup()
    {
        m_keyboardInput.Cleanup();
        m_gamepadInput.Cleanup();
    }

    public void SetControlMode(ControlMode mode)
    {
        m_currentMode = mode;
    }

    public Vector2Int DirectionFour()
    {
        Vector2Int vec2i = Vector2Int.zero;

        switch (m_currentMode)
        {
            case ControlMode.Keyboard:
                return m_keyboardInput.DirectionFour();

            case ControlMode.Gamepad:
                return m_gamepadInput.DirectionFour();

            case ControlMode.MultiKeyboardPriority:
                vec2i = m_keyboardInput.DirectionFour();
                if (vec2i == Vector2Int.zero)
                    vec2i = m_gamepadInput.DirectionFour();
                return vec2i;

            case ControlMode.MultiGamepadPriority:
                vec2i = m_gamepadInput.DirectionFour();
                if (vec2i == Vector2Int.zero)
                    vec2i = m_keyboardInput.DirectionFour();
                return vec2i;

            default:
                return Vector2Int.zero;
        }
    }

    public Vector2Int DirectionEight()
    {
        Vector2Int vec2i = Vector2Int.zero;

        switch (m_currentMode)
        {
            case ControlMode.Keyboard:
                return m_keyboardInput.DirectionEight();

            case ControlMode.Gamepad:
                return m_gamepadInput.DirectionEight();

            case ControlMode.MultiKeyboardPriority:
                vec2i = m_keyboardInput.DirectionEight();
                if (vec2i == Vector2Int.zero)
                    vec2i = m_gamepadInput.DirectionEight();
                return vec2i;

            case ControlMode.MultiGamepadPriority:
                vec2i = m_gamepadInput.DirectionEight();
                if (vec2i == Vector2Int.zero)
                    vec2i = m_keyboardInput.DirectionEight();
                return vec2i;

            default:
                return Vector2Int.zero;
        }
    }
}

internal abstract class AbstractInput
{
    protected PlayerControls m_input = null;

    public AbstractInput(PlayerControls input)
    {
        m_input = input;
    }

    public abstract void Init();

    public abstract void Cleanup();

    public abstract Vector2Int DirectionFour();

    public abstract Vector2Int DirectionEight();
}

internal class KeyboardInput : AbstractInput
{
    public KeyboardInput(PlayerControls input) : base(input)
    {
    }

    private enum ActiveInputs
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    private List<ActiveInputs> m_keyboardStack = new List<ActiveInputs>();

    public override void Init()
    {
        m_input.MoveKeyboard.MoveNorth.started += KeyboardNorthPress;
        m_input.MoveKeyboard.MoveNorth.canceled += KeyboardNorthRelease;

        m_input.MoveKeyboard.MoveEast.started += KeyboardEastPress;
        m_input.MoveKeyboard.MoveEast.canceled += KeyboardEastRelease;

        m_input.MoveKeyboard.MoveSouth.started += KeyboardSouthPress;
        m_input.MoveKeyboard.MoveSouth.canceled += KeyboardSouthRelease;

        m_input.MoveKeyboard.MoveWest.started += KeyboardWestPress;
        m_input.MoveKeyboard.MoveWest.canceled += KeyboardWestRelease;
    }

    public override void Cleanup()
    {
        m_input.MoveKeyboard.MoveNorth.started -= KeyboardNorthPress;
        m_input.MoveKeyboard.MoveNorth.canceled -= KeyboardNorthRelease;

        m_input.MoveKeyboard.MoveEast.started -= KeyboardEastPress;
        m_input.MoveKeyboard.MoveEast.canceled -= KeyboardEastRelease;

        m_input.MoveKeyboard.MoveSouth.started -= KeyboardSouthPress;
        m_input.MoveKeyboard.MoveSouth.canceled -= KeyboardSouthRelease;

        m_input.MoveKeyboard.MoveWest.started -= KeyboardWestPress;
        m_input.MoveKeyboard.MoveWest.canceled -= KeyboardWestRelease;
    }

    public override Vector2Int DirectionFour()
    {
        if (m_keyboardStack.Count == 0)
        { return Vector2Int.zero; }

        switch (m_keyboardStack[m_keyboardStack.Count - 1])
        {
            case ActiveInputs.North:
                return Vector2Int.up;

            case ActiveInputs.East:
                return Vector2Int.right;

            case ActiveInputs.South:
                return Vector2Int.down;

            case ActiveInputs.West:
                return Vector2Int.left;

            default:
                return Vector2Int.zero;
        }
    }

    public override Vector2Int DirectionEight()
    {
        Vector2Int vec2i = Vector2Int.zero;

        for (int i = 0; i < m_keyboardStack.Count; i++)
        {
            switch (m_keyboardStack[i])
            {
                case ActiveInputs.North:
                    vec2i += Vector2Int.up;
                    continue;
                case ActiveInputs.East:
                    vec2i += Vector2Int.right;
                    continue;
                case ActiveInputs.South:
                    vec2i += Vector2Int.down;
                    continue;
                case ActiveInputs.West:
                    vec2i += Vector2Int.left;
                    continue;
                default:
                    continue;
            }
        }

        return vec2i;
    }

    private void KeyboardNorthPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.North);

    private void KeyboardEastPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.East);

    private void KeyboardWestPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.West);

    private void KeyboardSouthPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.South);

    private void KeyboardNorthRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.North);

    private void KeyboardEastRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.East);

    private void KeyboardWestRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.West);

    private void KeyboardSouthRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.South);
}

internal class GamepadInput : AbstractInput
{
    private Vector2Int m_direction = Vector2Int.zero;
    private Vector2Int m_trueDirection = Vector2Int.zero;

    private const float m_kthreshold = 0.15f;

    public GamepadInput(PlayerControls input) : base(input)
    {
    }

    public override void Init()
    {
        m_input.MoveGamepad.Direction.performed += GamepadPerformed;
        m_input.MoveGamepad.Direction.canceled += GamepadCanceled;
    }

    public override void Cleanup()
    {
        m_input.MoveGamepad.Direction.performed -= GamepadPerformed;
        m_input.MoveGamepad.Direction.canceled += GamepadCanceled;
    }

    public override Vector2Int DirectionFour()
    {
        return m_direction;
    }

    public override Vector2Int DirectionEight()
    {
        return m_trueDirection;
    }

    private void GamepadPerformed(InputAction.CallbackContext context)
    {
        Vector2 vec2 = context.ReadValue<Vector2>();

        m_direction = Vector2Int.zero;
        m_trueDirection = Vector2Int.zero;

        if (vec2.magnitude < m_kthreshold)
        {
            return;
        }

        vec2 = vec2.normalized;

        // set m_direction
        if (Mathf.Abs(vec2.x) > Mathf.Abs(vec2.y))
        {
            if (vec2.x > 0)
            {
                m_direction = Vector2Int.right;
            }
            else
            {
                m_direction = Vector2Int.left;
            }
        }
        else
        {
            if (vec2.y > 0)
            {
                m_direction = Vector2Int.up;
            }
            else
            {
                m_direction = Vector2Int.down;
            }
        }

        // tan(22.5[deg]) = 0.2929

        if (Mathf.Abs(vec2.x) < 0.2929f)
        { vec2.x = 0f; }

        if (Mathf.Abs(vec2.y) < 0.2929f)
        { vec2.y = 0f; }

        if (vec2.x > 0)
        {
            m_trueDirection += Vector2Int.right;
        }
        else if (vec2.x < 0)
        {
            m_trueDirection += Vector2Int.left;
        }
        else
        {
        }

        if (vec2.y > 0)
        {
            m_trueDirection += Vector2Int.up;
        }
        else if (vec2.y < 0)
        {
            m_trueDirection += Vector2Int.down;
        }
        else
        {
        }
    }

    private void GamepadCanceled(InputAction.CallbackContext context)
    {
        m_direction = Vector2Int.zero;
        m_trueDirection = Vector2Int.zero;
    }
}