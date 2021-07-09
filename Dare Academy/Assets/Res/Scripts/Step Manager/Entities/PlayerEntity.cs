#define PLAYERENTITY_HOLD_FOR_ABILITY_MODE

using UnityEngine;
using blu;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using flags = GridEntityFlags.Flags;
using AbilityEnum = PlayerAbilities.AbilityEnum;

public class PlayerEntity : GridEntity
{
    // ABILITIES

    [SerializeField] private PlayerAbilities m_abilities = new PlayerAbilities();
    [SerializeField] private bool m_abilityMode = false;
    [SerializeField] private bool m_useAbility = false;

    public PlayerAbilities Abilities
    {
        get => m_abilities;
        set => m_abilities = value;
    }

    // HEALTH

    private int m_maxHealth = 1;
    public int MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }

    // ENERGY

    [SerializeField] private int m_currentEnergy = 0;
    private int m_maxEnergy = 3;

    public int Energy { get => m_currentEnergy; set => m_currentEnergy = value; }
    public int MaxEnergy { get => m_maxEnergy; set => m_maxEnergy = value; }

    private int m_dashEnergyCost = 3;
    private int m_shootEnergyCost = 3;
#pragma warning disable  CS0414// variable assigned but never used
    private int m_blockEnergyCost = 3;
#pragma warning restore  CS0414

    // OTHER

    private PlayerControls input;
    private GameObject m_bulletPrefab = null;
#pragma warning disable CS0414 // variable assigned but never used
    private int m_dashDistance = 2;
#pragma warning restore CS0414

    private Vector2Int m_moveDirection = Vector2Int.zero;
    private Vector2Int m_abilityDirection = Vector2Int.zero;

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

#if PLAYERENTITY_HOLD_FOR_ABILITY_MODE
        input.Ability.AbilityMode.started += EnterAbilityMode;
        input.Ability.AbilityMode.canceled += ExitAbilityMode;
#else
        input.Ability.AbilityMode.started += ToggleAbilityMode;
#endif

        input.Ability.CancelAbility.performed += CancelAbility;

        input.Ability.SwapAbilityL.performed += CycleAbilityL;
        input.Ability.SwapAbilityR.performed += CycleAbilityR;
    }

    private void OnDisable()
    {
        input.Move.Direction.started -= MovePressed;
        input.Move.Direction.canceled -= MoveReleased;

        input.Ability.Direction.performed -= AbilityDirection;

#if PLAYERENTITY_HOLD_FOR_ABILITY_MODE
        input.Ability.AbilityMode.started -= EnterAbilityMode;
        input.Ability.AbilityMode.canceled -= ExitAbilityMode;
#else
        input.Ability.AbilityMode.started -= ToggleAbilityMode;
#endif

        input.Ability.CancelAbility.performed -= CancelAbility;

        input.Ability.SwapAbilityL.performed -= CycleAbilityL;
        input.Ability.SwapAbilityR.performed -= CycleAbilityR;
    }

    protected void FixedUpdate()
    {
        if (m_abilityMode)
            return;

        if (m_useAbility && Abilities.GetActiveAbility() == AbilityEnum.Dash)
        {
            m_useAbility = false;
            if (Dash())
            {
                SetMovementDirection(m_abilityDirection, 2);
                m_abilityDirection = Vector2Int.zero;

                App.GetModule<LevelModule>().StepController.ExecuteStep();
                return;
            }
        }

        if (m_moveDirection != Vector2Int.zero)
        {
            SetMovementDirection(m_moveDirection, 1);
            App.GetModule<LevelModule>().StepController.ExecuteStep();
        }
    }

    protected void MovePressed(InputAction.CallbackContext context)
    {
        Vector2 vec2 = context.ReadValue<Vector2>();
        m_moveDirection = new Vector2Int(Mathf.RoundToInt(vec2.x), Mathf.RoundToInt(vec2.y));
    }

    protected void MoveReleased(InputAction.CallbackContext context)
    {
        m_moveDirection = Vector2Int.zero;
        SetMovementDirection(Vector2Int.zero);
    }

    protected void ToggleAbilityMode(InputAction.CallbackContext context) => m_abilityMode = !m_abilityMode;

    protected void EnterAbilityMode(InputAction.CallbackContext context) => m_abilityMode = true;

    protected void ExitAbilityMode(InputAction.CallbackContext context) => m_abilityMode = false;

    protected void CancelAbility(InputAction.CallbackContext context)
    {
        m_abilityMode = false;
        m_useAbility = false;
    }

    protected void CycleAbilityR(InputAction.CallbackContext context)
    {
        m_abilities.SetActiveAbility(m_abilities.RightAbility());
    }

    protected void CycleAbilityL(InputAction.CallbackContext context)
    {
        m_abilities.SetActiveAbility(m_abilities.LeftAbility());
    }

    protected void AbilityDirection(InputAction.CallbackContext context)
    {
        if (!m_abilityMode)
            return;

        m_useAbility = true;

        Vector2 vec2 = context.ReadValue<Vector2>();
        m_abilityDirection = new Vector2Int(Mathf.RoundToInt(vec2.x), Mathf.RoundToInt(vec2.y));
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
        // if (m_useAbility && m_abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot)
        // {
        //     m_useAbility = false;
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

        m_abilityDirection = Vector2Int.zero;
    }

    private bool Dash()
    {
        if (Energy >= m_dashEnergyCost)
        {
            if (m_abilityDirection != Vector2.zero)
            {
                Energy -= m_dashEnergyCost;
                return true;
            }
        }
        return false;
    }
}