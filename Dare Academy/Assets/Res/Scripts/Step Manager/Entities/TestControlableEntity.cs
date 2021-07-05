using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.InputSystem;

public class TestControlableEntity : GridEntity
{
    [SerializeField] private Vector2 testDirection = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    private PlayerControls input;

    protected override void Start()
    {
        base.Start();

        input = App.GetModule<InputModule>().PlayerController;

        input.Move.Direction.performed += SetDirection;
    }

    public void SetDirection(InputAction.CallbackContext context)
    {
        testDirection = context.ReadValue<Vector2>();
        // m_movementDirection = testDirection;
    }

    public override void AnalyseStep()
    {
        SetMovementDirection(testDirection, moveSpeed);
    }

    public override void EndStep()
    {
        base.EndStep();

        if (m_currentNode.overridden && m_currentNode.overrideType == NodeOverrideType.SceneConnection)
        {
            // transition to a new scene
            App.GetModule<SceneModule>().SwitchScene(
                m_currentNode.lvlTransitionInfo.targetSceneName,
                TransitionType.Slice,
                LoadingBarType.BottomBar
                );
        }

        if (m_currentNode != null)
        {
            if (m_currentNode.roomIndex != m_roomIndex)
            {
                m_roomIndex = m_currentNode.roomIndex;
                m_stepController.m_targetRoomIndex = m_roomIndex;
                Debug.Log("we have changed room");
                // we have changed room
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, testDirection);

        //Gizmos.color = Color.white;
        //Gizmos.DrawRay(transform.position, m_movementDirection);
    }
}