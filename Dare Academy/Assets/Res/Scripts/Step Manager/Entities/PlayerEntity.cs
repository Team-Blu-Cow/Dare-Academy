using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.InputSystem;
using JUtil;

public class PlayerEntity : GridEntity
{
    [SerializeField] private int moveSpeed = 1;
    private GameObject m_bulletPrefab = null;

    private Vector2 m_shootDirection = Vector2.zero;

    private PlayerControls input;

    public void OnValidate()
    {
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        input = App.GetModule<InputModule>().PlayerController;
        input.Move.Direction.performed += MoveAction;
        input.Aim.Direction.performed += ShootAction;
    }

    private void OnDisable()
    {
        input.Move.Direction.performed -= MoveAction;
        input.Aim.Direction.performed -= ShootAction;
    }

    protected void MoveAction(InputAction.CallbackContext context)
    {
        SetMovementDirection(context.ReadValue<Vector2>());
        App.GetModule<LevelModule>().StepController.ExecuteStep();
    }

    protected void ShootAction(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (m_shootDirection == Vector2.zero)
        {
            m_shootDirection = dir;
            return;
        }
        else
        {
            if (m_shootDirection == dir)
            {
                //cancel shot
                m_shootDirection = Vector2.zero;
            }
            else
            {
                m_shootDirection = dir;
            }
        }
    }

    public override void EndStep()
    {
        base.EndStep();

        if (m_currentNode.overridden && m_currentNode.overrideType == JUtil.Grids.NodeOverrideType.SceneConnection)
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

    public override void AttackStep()
    {
        if (m_shootDirection != Vector2.zero)
        {
            GridNode node;
            if (m_previousNode != null)
            {
                node = m_previousNode;

                if (Direction == m_shootDirection)
                {
                    node = m_currentNode;
                }
            }
            else
            {
                // player didn't move
                node = m_currentNode;
            }

            SpawnBullet(m_bulletPrefab, node, m_shootDirection);
            m_shootDirection = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_shootDirection != Vector2.zero)
        {
            Vector3 gizmoRay = new Vector3(m_shootDirection.x, m_shootDirection.y, 0);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, gizmoRay);
        }

        //Gizmos.color = Color.white;
        //Gizmos.DrawRay(transform.position, m_movementDirection);
    }
}