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
    // ABILITIES

    private bool m_abilityMode = false;
    private bool m_useAbility = false;
    private PlayerAbilities m_abilities = new PlayerAbilities();

    // HEALTH

    private int m_maxHealth = 1;
    public int MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }

    // ENERGY

    private int m_currentEnergy = 0;
    private int m_maxEnergy = 3;

    public int Energy { get => m_currentEnergy; set => m_currentEnergy = value; }
    public int MaxEnergy { get => m_maxEnergy; set => m_maxEnergy = value; }

    private int m_dashEnergyCost = 3;
    private int m_shootEnergyCost = 3;
    private int m_blockEnergyCost = 3;

    // OTHER

    private PlayerControls input;
    private GameObject m_bulletPrefab = null;
    private int m_dashDistance = 2;
    private Vector2 m_moveDirection = Vector2.zero;
    private Vector2 m_abilityDirection = Vector2.zero;

    public void OnValidate()
    {
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
    }

    protected override void Start()
    {
        base.Start();
        Energy = MaxEnergy;
        Health = MaxHealth;
        m_abilities.Initialise();
    }

    private void OnEnable()
    {
        input = App.GetModule<InputModule>().PlayerController;
        input.Move.Direction.started += MovePressed;
        input.Move.Direction.canceled += MoveReleased;

        input.Ability.Direction.performed += AbilityDirection;

        input.Ability.AbilityMode.started += EnterAbilityMode;
        input.Ability.AbilityMode.canceled += ExitAbilityMode;

        input.Ability.CancelAbility.performed += CancelAbility;

        input.Ability.SwapAbilityL.performed += CycleAbilityL;
        input.Ability.SwapAbilityR.performed += CycleAbilityR;
    }

    private void OnDisable()
    {
        input.Move.Direction.started -= MovePressed;
        input.Move.Direction.canceled -= MoveReleased;

        input.Ability.Direction.performed -= AbilityDirection;

        input.Ability.AbilityMode.started -= EnterAbilityMode;
        input.Ability.AbilityMode.canceled -= ExitAbilityMode;

        input.Ability.CancelAbility.performed -= CancelAbility;

        input.Ability.SwapAbilityL.performed -= CycleAbilityL;
        input.Ability.SwapAbilityR.performed -= CycleAbilityR;
    }

    protected void FixedUpdate()
    {
        if (m_moveDirection != Vector2Int.zero)
        {
            int speed = 1;
            // if (ActiveAbility == ActiveAbilityEnum.Dash)
            // {
            //     speed = Dash();
            // }

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

    protected void AbilityAction(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (m_abilityDirection == Vector2.zero)
        {
            m_abilityDirection = dir;
            return;
        }
        else
        {
            if (m_abilityDirection == dir)
            {
                //cancel ability
                m_abilityDirection = Vector2.zero;
            }
            else
            {
                m_abilityDirection = dir;
            }
        }
    }

    protected void EnterAbilityMode(InputAction.CallbackContext context)
    {
    }

    protected void ExitAbilityMode(InputAction.CallbackContext context)
    {
    }

    protected void CancelAbility(InputAction.CallbackContext context)
    {
    }

    protected void CycleAbilityR(InputAction.CallbackContext context)
    {
    }

    protected void CycleAbilityL(InputAction.CallbackContext context)
    {
    }

    protected void AbilityDirection(InputAction.CallbackContext context)
    {
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
        // if (ActiveAbility == ActiveAbilityEnum.Shoot)
        // {
        //     Shoot();
        // }
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
        if (m_abilityDirection != Vector2.zero)
        {
            Vector3 gizmoRay = new Vector3(m_abilityDirection.x, m_abilityDirection.y, 0);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, gizmoRay);
        }
    }

    // HELPER METHODS

    private void Shoot()
    {
        if (Energy >= m_shootEnergyCost)
        {
            if (m_abilityDirection != Vector2.zero)
            {
                GridNode node;
                if (m_previousNode != null)
                {
                    node = m_previousNode;

                    if (Direction == m_abilityDirection)
                    {
                        node = m_currentNode;
                    }
                }
                else
                {
                    // player didn't move
                    node = m_currentNode;
                }

                if (SpawnBullet(m_bulletPrefab, node, m_abilityDirection))
                {
                    Energy -= m_shootEnergyCost;
                }
            }
        }

        m_abilityDirection = Vector2.zero;
    }

    private int Dash()
    {
        if (Energy >= m_dashEnergyCost)
        {
            if (m_abilityDirection != Vector2.zero)
            {
                Energy -= m_dashEnergyCost;
                return m_dashDistance;
            }
        }

        return 1;
    }
}