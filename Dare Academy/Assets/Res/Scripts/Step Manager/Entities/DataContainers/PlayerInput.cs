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
        Keyboard
    }

    private ControlMode m_currentMode = ControlMode.None;
    private PlayerControls m_input;
    private KeyboardInput m_keyboardInput;

    public void Init()
    {
        m_input = App.GetModule<InputModule>().PlayerController;
        m_keyboardInput = new KeyboardInput(m_input);
        m_keyboardInput.Init();
    }

    public void Cleanup()
    {
        m_keyboardInput.Cleanup();
    }

    public void SetControlMode(ControlMode mode)
    {
        m_currentMode = mode;
    }

    public Vector2Int Direction()
    {
        switch (m_currentMode)
        {
            case ControlMode.Keyboard:
                return m_keyboardInput.Direction();

            case ControlMode.Gamepad:
                return Vector2Int.zero;

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

    public abstract Vector2Int Direction();
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

    public override Vector2Int Direction()
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

    private void KeyboardNorthPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.North);

    private void KeyboardEastPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.East);

    private void KeyboardWestPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.West);

    private void KeyboardSouthPress(InputAction.CallbackContext context) => m_keyboardStack.Add(ActiveInputs.South);

    private void KeyboardNorthRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.North);

    private void KeyboardEastRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.East);

    private void KeyboardWestRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.West);

    private void KeyboardSouthRelease(InputAction.CallbackContext context) => m_keyboardStack.Remove(ActiveInputs.South);
}