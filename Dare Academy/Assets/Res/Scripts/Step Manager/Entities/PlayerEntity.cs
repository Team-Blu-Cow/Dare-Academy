#define PLAYERENTITY_HOLD_FOR_ABILITY_MODE

using UnityEngine;
using blu;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;
using AbilityEnum = PlayerAbilities.AbilityEnum;

public class PlayerEntity : GridEntity
{
    // ABILITIES

    [SerializeField] private PlayerAbilities m_abilities = new PlayerAbilities();
    [SerializeField] private bool m_abilityMode = false;

    // if the scene switch has been triggered but the entity has not been destroyed
    private bool m_sceneHasSwitched = false;

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
    private int m_blockEnergyCost = 3;

    // OTHER

    private PlayerControls m_input;
    private GameObject m_bulletPrefab = null;
    private int m_dashDistance = 2;

    private PlayerInput m_playerInput = new PlayerInput();

    private Vector2Int m_moveDirection = Vector2Int.zero;
    private Vector2Int m_abilityDirection = Vector2Int.zero;

    protected override void OnValidate()
    {
        base.OnValidate();
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
    }

    protected void Awake()
    {
    }

    protected override void Start()
    {
        base.Start();
        Energy = MaxEnergy;
        Health = MaxHealth;
        m_abilities.Initialise();
        m_animationController = GetComponent<GridEntityAnimationController>();
        m_playerInput.SetControlMode(PlayerInput.ControlMode.Keyboard);
    }

    private void OnEnable()
    {
        m_input = App.GetModule<InputModule>().PlayerController;

        m_playerInput.Init();

#if PLAYERENTITY_HOLD_FOR_ABILITY_MODE
        m_input.Ability.AbilityMode.started += EnterAbilityMode;
        m_input.Ability.AbilityMode.canceled += ExitAbilityMode;
#else
        input.Ability.AbilityMode.started += ToggleAbilityMode;
#endif

        m_input.Ability.CancelAbility.performed += CancelAbility;

        m_input.Ability.SwapAbilityL.performed += CycleAbilityL;
        m_input.Ability.SwapAbilityR.performed += CycleAbilityR;
    }

    private void OnDisable()
    {
        m_playerInput.Cleanup();

#if PLAYERENTITY_HOLD_FOR_ABILITY_MODE
        m_input.Ability.AbilityMode.started -= EnterAbilityMode;
        m_input.Ability.AbilityMode.canceled -= ExitAbilityMode;
#else
        input.Ability.AbilityMode.started -= ToggleAbilityMode;
#endif

        m_input.Ability.CancelAbility.performed -= CancelAbility;

        m_input.Ability.SwapAbilityL.performed -= CycleAbilityL;
        m_input.Ability.SwapAbilityR.performed -= CycleAbilityR;
    }

    protected void Update()
    {
        if (m_abilityMode)
        {
            Vector2Int direction = m_playerInput.TrueDirection();

            float headX = 0;
            float headY = 0;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                headX = direction.x;
            else
                headY = direction.y;

            m_animationController.SetHeadDirection(headX, headY);
        }

        m_moveDirection = Vector2Int.zero;
        m_abilityDirection = Vector2Int.zero;

        if (m_abilityMode)
        {
            m_abilityDirection = m_playerInput.TrueDirection();
        }
        else
        {
            m_moveDirection = m_playerInput.Direction();
        }

        if (Abilities.GetActiveAbility() == AbilityEnum.Dash && !m_abilityMode && m_abilityDirection != Vector2Int.zero)
        {
            if (Dash())
            {
                SetMovementDirection(m_abilityDirection, m_dashDistance);
                m_abilityDirection = Vector2Int.zero;
                App.GetModule<LevelModule>().StepController.ExecuteStep();
            }
            else
            {
                // failed to dash, no enough energy
                // TODO @matthew/@adam - sound effect here
                m_abilityDirection = Vector2Int.zero;
            }
        }

        if (Abilities.GetActiveAbility() == AbilityEnum.Block && !m_abilityMode && m_abilityDirection != Vector2Int.zero)
        {
            if (Block())
            {
                SetMovementDirection(Vector2Int.zero, 0);
                m_abilityDirection = Vector2Int.zero;
                m_moveDirection = Vector2Int.zero;
                App.GetModule<LevelModule>().StepController.ExecuteStep();
            }
            else
            {
                // failed to dash, no enough energy
                // TODO @matthew/@adam - sound effect here
                m_abilityDirection = Vector2Int.zero;
            }
        }

        if (m_moveDirection != Vector2Int.zero)
        {
            SetMovementDirection(m_moveDirection, 1);
            m_moveDirection = Vector2Int.zero;
            if (!m_sceneHasSwitched)
                App.GetModule<LevelModule>().StepController.ExecuteStep();
        }
    }

    protected void SetAbilityAnimationFlag()
    {
        int animationFlag;

        switch (m_abilities.GetActiveAbility())
        {
            case AbilityEnum.None: animationFlag = 0; break;
            case AbilityEnum.Shoot: animationFlag = 1; break;
            case AbilityEnum.Dash: animationFlag = 2; break;
            case AbilityEnum.Block: animationFlag = 3; break;
            default: animationFlag = 0; break;
        }

        m_animationController.animator.SetInteger("AbilityState", animationFlag);
    }

    protected void SetAbilityAnimationFlag(int animationFlag)
    {
        m_animationController.animator.SetInteger("AbilityState", animationFlag);
    }

    public override void AnalyseStep()
    {
        if (m_moveDirection != Vector2.zero)
            m_animationController.SetDirection(m_moveDirection.x, 1);

        if (m_animationController != null)
            SetAbilityAnimationFlag(0);
    }

    protected void ToggleAbilityMode(InputAction.CallbackContext context)
    {
        m_abilityMode = !m_abilityMode;

        SetAbilityAnimationFlag();
    }

    protected void EnterAbilityMode(InputAction.CallbackContext context)
    {
        m_abilityMode = true;
        SetAbilityAnimationFlag();
    }

    protected void ExitAbilityMode(InputAction.CallbackContext context) => m_abilityMode = false;

    protected void CancelAbility(InputAction.CallbackContext context)
    {
        m_abilityMode = false;
    }

    protected void CycleAbilityR(InputAction.CallbackContext context)
    {
        m_abilities.SetActiveAbility(m_abilities.RightAbility());

        SetAbilityAnimationFlag();
    }

    protected void CycleAbilityL(InputAction.CallbackContext context)
    {
        m_abilities.SetActiveAbility(m_abilities.LeftAbility());
        SetAbilityAnimationFlag();
    }

    public override void EndStep()
    {
        base.EndStep();

        Energy++;

        if (Energy > MaxEnergy)
            Energy = MaxEnergy;

        if (Energy < 0)
            Energy = 0;

        if (m_currentNode.overridden)
        {
            if (m_currentNode.overrideType == NodeOverrideType.SceneConnection)
            {
                m_sceneHasSwitched = true;
                App.GetModule<LevelModule>().lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                // transition to a new scene
                App.GetModule<SceneModule>().SwitchScene(
                    m_currentNode.lvlTransitionInfo.targetSceneName,
                    m_currentNode.lvlTransitionInfo.transitionType,
                    m_currentNode.lvlTransitionInfo.loadType
                    );
            }

            if (m_currentNode.overrideType == NodeOverrideType.LostWoodsConnection)
            {
                m_sceneHasSwitched = true;
                // check lost woods count
                if (!App.GetModule<LevelModule>().persistantSceneData._switching)
                {
                    App.GetModule<LevelModule>().persistantSceneData._switching = true;

                    if (App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter == 0 && LastDirection == Vector2Int.down)
                    {
                        m_currentNode.lvlTransitionInfo.targetNodeIndex = new Vector2Int(2, 4);
                        m_currentNode.lvlTransitionInfo.targetSceneName = "Mushroom Forest Start";
                        m_currentNode.lvlTransitionInfo.targetRoomIndex = 8;

                        App.GetModule<LevelModule>().lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                        Destroy(App.GetModule<LevelModule>().persistantSceneData._soundEmitter);
                        App.GetModule<LevelModule>().persistantSceneData = new PersistantSceneData();

                        App.GetModule<SceneModule>().SwitchScene(
                            m_currentNode.lvlTransitionInfo.targetSceneName,
                            m_currentNode.lvlTransitionInfo.transitionType,
                            m_currentNode.lvlTransitionInfo.loadType
                            );
                    }
                    else if (App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter >= 3)
                    {
                        m_currentNode.lvlTransitionInfo.targetNodeIndex = new Vector2Int(0, 3);
                        m_currentNode.lvlTransitionInfo.targetSceneName = "Mushroom Forest Start";
                        m_currentNode.lvlTransitionInfo.targetRoomIndex = 3;
                        App.GetModule<LevelModule>().lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                        Destroy(App.GetModule<LevelModule>().persistantSceneData._soundEmitter);
                        App.GetModule<LevelModule>().persistantSceneData = new PersistantSceneData();

                        App.GetModule<SceneModule>().SwitchScene(
                        m_currentNode.lvlTransitionInfo.targetSceneName,
                        m_currentNode.lvlTransitionInfo.transitionType,
                        m_currentNode.lvlTransitionInfo.loadType
                        );
                    }
                    else
                    {
                        App.GetModule<LevelModule>().lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                        if (LastDirection == App.GetModule<LevelModule>().persistantSceneData._direction)
                        {
                            App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter++;
                            Debug.Log(App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter);
                        }
                        else
                        {
                            App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter = 0;
                            Debug.Log(App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter);
                        }

                        App.GetModule<SceneModule>().SwitchScene(
                        m_currentNode.lvlTransitionInfo.targetSceneName,
                        m_currentNode.lvlTransitionInfo.transitionType,
                        m_currentNode.lvlTransitionInfo.loadType
                        );
                    }
                }
            }
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
        if (!m_abilityMode && m_abilities.GetActiveAbility() == AbilityEnum.Shoot)
        {
            Shoot();
        }
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
                m_internalFlags.SetFlags(interalFlags.isDead, false);
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

    protected override void OnDrawGizmos()
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

                AddAnimationAction(ActionTypes.STATIC_ACTION, "Head Gun Fire", 1);
                SetAbilityAnimationFlag(0);

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

    private bool Block()
    {
        if (Energy >= m_blockEnergyCost)
        {
            if (m_abilityDirection != Vector2.zero)
            {
                Energy -= m_blockEnergyCost;

                AddAnimationAction(ActionTypes.STATIC_ACTION, "Head Shield Fire", 1);
                SetAbilityAnimationFlag(0);

                System.Func<Vector2Int, interalFlags, bool> BlockCheckDirectionAndSetFlag = (vec, flag) =>
                {
                    if (vec == m_abilityDirection)
                    {
                        m_internalFlags.SetFlags(flag, true);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };

                if (BlockCheckDirectionAndSetFlag(Vector2Int.up, interalFlags.refectBullets_N))
                { return true; }

                if (BlockCheckDirectionAndSetFlag(Vector2Int.right, interalFlags.refectBullets_E))
                { return true; }

                if (BlockCheckDirectionAndSetFlag(Vector2Int.down, interalFlags.refectBullets_S))
                { return true; }

                if (BlockCheckDirectionAndSetFlag(Vector2Int.left, interalFlags.refectBullets_W))
                { return true; }

                Vector2Int vecDir = new Vector2Int(1, 1);
                if (BlockCheckDirectionAndSetFlag(vecDir, interalFlags.refectBullets_NE))
                { return true; }

                vecDir = new Vector2Int(-1, 1);
                if (BlockCheckDirectionAndSetFlag(vecDir, interalFlags.refectBullets_NW))
                { return true; }

                vecDir = new Vector2Int(1, -1);
                if (BlockCheckDirectionAndSetFlag(vecDir, interalFlags.refectBullets_SE))
                { return true; }

                vecDir = new Vector2Int(-1, -1);
                if (BlockCheckDirectionAndSetFlag(vecDir, interalFlags.refectBullets_SW))
                { return true; }
            }
        }

        return false;
    }
}