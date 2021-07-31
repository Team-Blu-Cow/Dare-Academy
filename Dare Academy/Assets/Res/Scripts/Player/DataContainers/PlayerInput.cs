using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class PlayerInput
{
    private PlayerControls m_input;
    private Vector2Int m_directionFour = Vector2Int.zero;
    private Vector2Int m_directionEight = Vector2Int.zero;
    private const float m_kthreshold = 0.15f;
    private bool m_ignoreUntilInputZero = false;

    public void Init()
    {
        m_input = App.GetModule<InputModule>().PlayerController;

        m_input.Player.Direction.performed += DirectionPerformed;
        m_input.Player.Direction.canceled += DirectionCanceled;
    }

    public void Cleanup()
    {
        m_input.Player.Direction.performed -= DirectionPerformed;
        m_input.Player.Direction.canceled -= DirectionCanceled;
    }

    public void IgnoreInputUntilZero() => m_ignoreUntilInputZero = true;

    public Vector2Int DirectionFour(bool forceResult = false)
    {
        if (m_ignoreUntilInputZero && !forceResult)
            return Vector2Int.zero;
        else
            return m_directionFour;
    }

    public Vector2Int DirectionEight(bool forceResult = false)
    {
        if (m_ignoreUntilInputZero && !forceResult)
            return Vector2Int.zero;
        else
            return m_directionEight;
    }

    private void DirectionPerformed(InputAction.CallbackContext context)
    {
        Vector2 vec2 = context.ReadValue<Vector2>();

        m_directionFour = Vector2Int.zero;
        m_directionEight = Vector2Int.zero;

        if (vec2.magnitude < m_kthreshold)
            return;

        vec2 = vec2.normalized;

        // set m_directionFour
        if (Mathf.Abs(vec2.x) > Mathf.Abs(vec2.y))
        {
            if (vec2.x > 0)
            {
                m_directionFour = Vector2Int.right;
            }
            else
            {
                m_directionFour = Vector2Int.left;
            }
        }
        else
        {
            if (vec2.y > 0)
            {
                m_directionFour = Vector2Int.up;
            }
            else
            {
                m_directionFour = Vector2Int.down;
            }
        }

        // tan(22.5[deg]) = 0.2929

        if (Mathf.Abs(vec2.x) < 0.2929f)
        { vec2.x = 0f; }

        if (Mathf.Abs(vec2.y) < 0.2929f)
        { vec2.y = 0f; }

        if (vec2.x > 0)
        {
            m_directionEight += Vector2Int.right;
        }
        else if (vec2.x < 0)
        {
            m_directionEight += Vector2Int.left;
        }
        else
        {
        }

        if (vec2.y > 0)
        {
            m_directionEight += Vector2Int.up;
        }
        else if (vec2.y < 0)
        {
            m_directionEight += Vector2Int.down;
        }
        else
        {
        }
    }

    private void DirectionCanceled(InputAction.CallbackContext context)
    {
        m_directionFour = Vector2Int.zero;
        m_directionEight = Vector2Int.zero;
        m_ignoreUntilInputZero = false;
    }
}