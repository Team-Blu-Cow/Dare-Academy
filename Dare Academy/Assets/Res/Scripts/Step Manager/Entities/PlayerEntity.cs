using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.InputSystem;
using JUtil;
using UnityEngine.SceneManagement;
using flags = GridEntityFlags.Flags;

public class PlayerEntity : GridEntity
{
    private GameObject m_bulletPrefab = null;
    private Vector2 m_shootDirection = Vector2.zero;

    [SerializeField] private int m_maxEnergy = 3;
    private int m_currentEnergy = 0;

    private bool m_shouldDash = false; // flag for if the player is dashing this step
    private int m_dashDistance = 2;
    private int m_dashEnergyCost = 3;

    private int m_shootEnergyCost = 3;

    private int m_maxHealth = 1;
    public int MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }

    [SerializeField] private bool m_allowDash = false; // if the player is allowed to dash
    public int Energy { get => m_currentEnergy; set => m_currentEnergy = value; }
    public int MaxEnergy { get => m_maxEnergy; set => m_maxEnergy = value; }

    // the movement direction stored within GridEntity is cleared every step so we store another copy here
    private Vector2 m_moveDirection = Vector2.zero;

    private PlayerControls input;

    public void OnValidate()
    {
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
    }

    protected override void Start()
    {
        base.Start();
        Energy = MaxEnergy;
        Health = MaxHealth;
    }

    private void OnEnable()
    {
        input = App.GetModule<InputModule>().PlayerController;
        input.Move.Direction.started += MovePressed;
        input.Move.Direction.canceled += MoveReleased;
        input.Aim.Direction.performed += ShootAction;
        input.Move.Dash.performed += DashPressed;
    }

    private void OnDisable()
    {
        input.Move.Direction.started -= MovePressed;
        input.Move.Direction.canceled -= MoveReleased;
        input.Aim.Direction.performed -= ShootAction;
        input.Move.Dash.performed += DashPressed;
    }

    protected void FixedUpdate()
    {
        if (m_moveDirection != Vector2Int.zero)
        {
            int speed = 1;
            if (m_allowDash && m_shouldDash)
            {
                speed = Dash();
            }

            SetMovementDirection(m_moveDirection, speed);
            App.GetModule<LevelModule>().StepController.ExecuteStep();
        }
    }

    protected void MovePressed(InputAction.CallbackContext context)
    {
        m_moveDirection = context.ReadValue<Vector2>();
    }

    protected void MoveReleased(InputAction.CallbackContext context)
    {
        m_moveDirection = Vector2Int.zero;
        SetMovementDirection(Vector2Int.zero);
    }

    protected void DashPressed(InputAction.CallbackContext context)
    {
        m_shouldDash = !m_shouldDash;
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

        Energy++;

        if (Energy > MaxEnergy)
            Energy = MaxEnergy;

        if (Energy < 0)
            Energy = 0;

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

    public override void AttackStep()
    {
        if (Energy >= m_shootEnergyCost)
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

                if (SpawnBullet(m_bulletPrefab, node, m_shootDirection))
                {
                    Energy -= m_shootEnergyCost;
                }
            }
        }

        m_shootDirection = Vector2.zero;
    }

    public override void OnDeath()
    {
        RespawnStationEntity respawnStation = RespawnStationEntity.CurrentRespawnStation;
        if (respawnStation != null)
        {
            GridNode respawnPoint = respawnStation.RespawnLocation();
            if (respawnPoint != null)
            {
                RemoveFromCurrentNode();
                m_currentNode = respawnPoint;
                m_flags.SetFlags(flags.isDead, false);
                Health = MaxHealth;
                Energy = MaxEnergy;
                transform.position = m_currentNode.position.world;
                AddToCurrentNode();
                return;
            }
        }

        Debug.LogWarning("Player failed to respawn, reloading scene");
        App.GetModule<SceneModule>().SwitchScene(SceneManager.GetActiveScene().name, TransitionType.LRSweep);
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

    // HELPER METHODS

    private int Dash()
    {
        m_shouldDash = false;
        if (Energy >= m_dashEnergyCost)
        {
            Energy -= m_dashEnergyCost;
            return m_dashDistance;
        }

        return 1;
    }
}