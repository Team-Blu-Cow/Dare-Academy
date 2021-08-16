using UnityEngine;
using blu;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using JUtil;
using JUtil.Grids;

using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;
using eventFlags = GameEventFlags.Flags;
using AbilityEnum = PlayerAbilities.AbilityEnum;
using System.Threading.Tasks;

public class PlayerEntity : GridEntity
{
    // benchmarking shows calling App.GetModule<> 200 times will cost us 1ms
    // caching this will negate this issue, due to how often its called in here we are caching all the modules

    private LevelModule levelModule;
    private SceneModule sceneModule;
    private AudioModule audioModule;
    private InputModule inputModule;

    // ABILITIES

    [SerializeField] private PlayerAbilities m_abilities = new PlayerAbilities();
    [SerializeField] private bool m_abilityMode = false;
    public GameObject m_interactToolTip;

    public bool LoadingFromOtherScene
    { get; set; }

    // if the scene switch has been triggered but the entity has not been destroyed
    private bool m_sceneHasSwitched = false;

    private static PlayerEntity _Instance;

    public static PlayerEntity Instance
    {
        get => _Instance;
    }

    public PlayerAbilities Abilities
    {
        get => m_abilities;
        set => m_abilities = value;
    }

    // HEALTH

    private int m_maxHealth = 3;
    public int MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }

    // ENERGY

    [SerializeField] private int m_currentEnergy = 0;
    private int m_maxEnergy = 3;

    public int Energy { get => m_currentEnergy; set => m_currentEnergy = value; }
    public int MaxEnergy { get => m_maxEnergy; set => m_maxEnergy = value; }

    public int m_dashEnergyCost;
    public int m_shootEnergyCost;
    public int m_blockEnergyCost;

    // INPUT

    public enum AbilityInputMode
    {
        None,
        Hold,
        Toggle,
    }

    public AbilityInputMode abilityInputMode
    { get; private set; }

    private PlayerInput m_playerInput = new PlayerInput();

    // OTHER
    private int m_pickups;

    public int ShipParts { get => m_pickups; set => m_pickups = value; }

    private PlayerControls m_input;
    [SerializeField] private GameObject m_bulletPrefab = null;
    private int m_dashDistance = 2;
    private bool m_abilityUsedThisTurn = false;

    private Vector2Int m_moveDirection = Vector2Int.zero;
    [SerializeField] public Vector2Int m_abilityDirection = Vector2Int.zero;

    [SerializeField] private int m_overlayRadius = 3;
    [SerializeField] private bool m_overlayToggle;

    public Dictionary<string, List<int>> m_dictRoomsTraveled = new Dictionary<string, List<int>>();

    public PlayerEntityAnimationController animationController
    {
        get { return ((PlayerEntityAnimationController)m_animationController); }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/PlayerLuvBullet");
    }

    protected void Awake()
    {
        if (_Instance != null)
        {
            Debug.LogWarning("[PlayerEntity] Instance already exists");
        }

        _Instance = this;

        levelModule = App.GetModule<LevelModule>();
        sceneModule = App.GetModule<SceneModule>();
        audioModule = App.GetModule<AudioModule>();
        inputModule = App.GetModule<InputModule>();

        levelModule.LoadFromSave();

        Abilities.Initialise();

        while (levelModule.ActiveSaveData == null) { }

        MaxHealth = levelModule.ActiveSaveData.maxHealth;
        MaxEnergy = levelModule.ActiveSaveData.maxEnergy;

        Health = levelModule.ActiveSaveData.currentHealth;
        Energy = levelModule.ActiveSaveData.currentEnergy;
    }

    public void DebugSetNode()
    {
        m_currentNode = levelModule.MetaGrid.GetNodeFromWorld(transform.position);

        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_roomIndex = m_currentNode.roomIndex;
    }

    protected override void Start()
    {
        abilityInputMode = AbilityInputMode.None;
        UpdateInputMode();

        if (levelModule.LevelManager.debug_SpawnPlayer && levelModule.ActiveSaveData.useRespawnData && !LoadingFromOtherScene)
        {
            Vector3 pos = levelModule.MetaGrid.Grid(levelModule.ActiveSaveData.respawnRoomID)[levelModule.ActiveSaveData.respawnLocation].position.world;
            gameObject.transform.position = pos;

            App.CameraController.Init(pos);
        }

        base.Start();
        levelModule.StepController.m_targetRoomIndex = RoomIndex;
        levelModule.StepController.CheckForRoomChange();

        inputModule.PlayerController.Player.Enable();
        inputModule.SystemController.UI.Map.Enable();

        m_animationController = GetComponent<GridEntityAnimationController>();

        StoreRespawnLoaction();

        foreach (Quest.StringListIntPair pair in levelModule.ActiveSaveData.m_roomsTraveled)
        {
            if (!m_dictRoomsTraveled.ContainsKey(pair.key))
                m_dictRoomsTraveled.Add(pair.key, pair.value);
        }

        // add starting room to minimap list
        if (m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name))
        {
            if (!m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(m_roomIndex))
                m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Add(m_roomIndex);
        }
        else
            m_dictRoomsTraveled.Add(SceneManager.GetActiveScene().name, new List<int> { m_roomIndex });

        SetInteractPosition();
        try
        {
            FindObjectOfType<MiniMapGen>().DrawMap();
        }
        catch
        {
            Debug.LogWarning("[PlayerEntity.Start] failed to draw MiniMap");
        }

        App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/MainQuest"));

        audioModule.PlayAudioEvent("event:/SFX/Player/sfx_ability_select");
        audioModule.PlayAudioEvent("event:/SFX/Player/sfx_dash_chrage");

        UpdateSaveFile();
    }

    private void OnDestroy()
    {
        UpdateSaveFile();
    }

    private void OnEnable()
    {
        m_input = inputModule.PlayerController;

        m_playerInput.Init();

        m_input.Player.AbilityMode.started += EnterAbilityMode;
        m_input.Player.AbilityMode.canceled += ExitAbilityMode;
        m_input.Player.AbilityMode.started += ToggleAbilityMode;

        m_input.Player.CancelAbility.performed += CancelAbility;

        m_input.Player.SwapAbilityL.performed += CycleAbilityR; // Cycles opposite way for Player Ui carousel // Can mabey change func names to not be weird
        m_input.Player.SwapAbilityR.performed += CycleAbilityL;
    }

    private void OnDisable()
    {
        m_playerInput.Cleanup();

        m_input.Player.AbilityMode.started -= EnterAbilityMode;
        m_input.Player.AbilityMode.canceled -= ExitAbilityMode;
        m_input.Player.AbilityMode.started -= ToggleAbilityMode;

        m_input.Player.CancelAbility.performed -= CancelAbility;

        m_input.Player.SwapAbilityL.performed -= CycleAbilityR;
        m_input.Player.SwapAbilityR.performed -= CycleAbilityL;
    }

    protected void Update()
    {
        m_moveDirection = Vector2Int.zero;
        // m_abilityDirection = Vector2Int.zero;

        if (!levelModule.IsSaveLoaded)
            return;

        if (levelModule.ActiveSaveData != null)
            levelModule.ActiveSaveData.playtime += Time.deltaTime;

        if (!LevelManager.Instance.AllowPlayerMovement)
            return;

        if (isDead)
            return;

        if (m_abilityMode)
        {
            Vector2Int direction;// = m_playerInput.DirectionEight(true);

            if (Abilities.GetActiveAbility() == AbilityEnum.Block)
            {
                direction = m_playerInput.DirectionEight(true);
            }
            else
            {
                direction = m_playerInput.DirectionFour(true);
            }

            if (direction != Vector2Int.zero)
            {
                animationController.SetAbilityState(GetAbilityStateInt());

                float headX = 0;
                float headY = 0;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    headX = direction.x;
                else
                    headY = direction.y;

                m_animationController.SetHeadDirection(headX, headY);

                m_abilityDirection = direction;
                m_playerInput.IgnoreInputUntilZero();
            }
        }
        else
        {
            m_moveDirection = m_playerInput.DirectionFour();
        }

        if (Abilities.GetActiveAbility() == AbilityEnum.Dash && !m_abilityMode && m_abilityDirection != Vector2Int.zero)
        {
            if (Dash())
            {
                animationController.StopDashChargeParticles();
                SetMovementDirection(m_abilityDirection, m_dashDistance);
                animationController.StartDashEffect(m_abilityDirection);
                m_abilityDirection = Vector2Int.zero;
                ExecuteStep();
            }
            else
            {
                // failed to dash, no enough energy
                blu.App.GetModule<AudioModule>().PlayAudioEvent("event:/SFX/Player/sfx_ability_fail");
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
                ExecuteStep();
            }
            else
            {
                // failed to block, no enough energy
                blu.App.GetModule<AudioModule>().PlayAudioEvent("event:/SFX/Player/sfx_ability_fail");
                m_abilityDirection = Vector2Int.zero;
            }
        }

        if (m_moveDirection != Vector2Int.zero)
        {
            SetMovementDirection(m_moveDirection, 1);
            m_moveDirection = Vector2Int.zero;
            if (!m_sceneHasSwitched)
                ExecuteStep();

            SetMovementDirection(Vector2Int.zero, 0);
        }
    }

    // run immediately before the step controller runs ExecuteStep()
    // returns if step should be executed
    public bool PreExecuteStep()
    {
        if (m_abilityMode && Abilities.GetActiveAbility() == AbilityEnum.Dash && m_abilityDirection != Vector2Int.zero)
        {
            _ExitAbilityMode();
            return false;
        }
        else
        {
            _ExitAbilityMode();
            return true;
        }
    }

    public void ExecuteStep()
    {
        levelModule.ExecuteStep();
    }

    protected void UpdateSaveFile()
    {
        levelModule.ActiveSaveData.m_roomsTraveled.Clear();
        foreach (var pair in m_dictRoomsTraveled)
            levelModule.ActiveSaveData.m_roomsTraveled.Add(new Quest.StringListIntPair(pair.Key, pair.Value));

        levelModule.ActiveSaveData.maxHealth = MaxHealth;
        levelModule.ActiveSaveData.maxEnergy = MaxEnergy;

        levelModule.ActiveSaveData.currentHealth = Health;
        levelModule.ActiveSaveData.currentEnergy = Energy;
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

        //m_animationController.animator.SetInteger("AbilityState", animationFlag);
        animationController.SetAbilityState(animationFlag);
    }

    protected void SetAbilityAnimationFlag(int animationFlag)
    {
        //m_animationController.animator.SetInteger("AbilityState", animationFlag);
    }

    public override void ResetAnimations()
    {
        base.ResetAnimations();
        LastDirection = Vector2Int.zero;
    }

    public override void AnalyseStep()
    {
        if (m_moveDirection != Vector2.zero)
            m_animationController.SetDirection(m_moveDirection.x, 1);

        if (m_animationController != null)
            SetAbilityAnimationFlag(0);
        DrawMoveOverlays();
    }

    public void DrawMoveOverlays()
    {
        m_overlayToggle = !m_overlayToggle;

        Grid<GridNode> currentGrid = levelModule.CurrentRoom;

        GridNodePosition startOffset = m_currentNode.position;
        int xx = startOffset.grid.x - m_overlayRadius;
        int yy = startOffset.grid.y - m_overlayRadius;

        int diameter = (m_overlayRadius * 2) + 1;

        int modVal = 0;

        if (m_overlayToggle)
            modVal = 1;

        for (int x = 0; x < diameter; x++)
        {
            yy = startOffset.grid.y - m_overlayRadius;
            for (int y = 0; y < diameter; y++)
            {
                if ((x + y) % 2 != modVal)
                {
                    GridNode node = currentGrid[xx, yy];
                    if (node != null && node.roomIndex == m_currentNode.roomIndex && node.IsTraversable(false))
                    {
                        float dist = Vector3.Distance(node.position.world, m_currentNode.position.world);

                        dist = ((m_overlayRadius - dist) / (m_overlayRadius)) / 2;

                        levelModule.telegraphDrawer.CreateTelegraph(node, TelegraphDrawer.Type.MOVE, dist);
                    }
                }
                yy++;
            }

            xx++;
        }
    }

    protected void ToggleAbilityMode(InputAction.CallbackContext context)
    {
        if (abilityInputMode != AbilityInputMode.Toggle)
            return;

        if (!m_abilityMode && !LevelManager.Instance.AllowPlayerMovement)
            return;

        if (m_abilityMode)
        {
            _ExitAbilityMode();
        }
        else
        {
            _EnterAbilityMode();
        }
    }

    protected void EnterAbilityMode(InputAction.CallbackContext context)
    {
        if (abilityInputMode != AbilityInputMode.Hold)
            return;

        if (!LevelManager.Instance.AllowPlayerMovement)
            return;

        _EnterAbilityMode();
    }

    protected void ExitAbilityMode(InputAction.CallbackContext context)
    {
        if (abilityInputMode != AbilityInputMode.Hold)
            return;

        _ExitAbilityMode();
    }

    protected void _EnterAbilityMode()
    {
        m_abilityMode = true;
        if (audioModule.GetCurrentSong() != null)
            audioModule.GetCurrentSong().SetParameter("Muffled", 1);
        ToggleAnimationState();
    }

    protected void _ExitAbilityMode()
    {
        m_abilityMode = false;

        if (!levelModule.LevelManager.paused && audioModule.GetCurrentSong() != null)
        {
            audioModule.GetCurrentSong().SetParameter("Muffled", 0);
        }

        animationController.DisableVignette();
        if (m_abilityDirection == Vector2Int.zero)
            animationController.SetAbilityState(0);
    }

    protected void CancelAbility(InputAction.CallbackContext context)
    {
        m_abilityDirection = Vector2Int.zero;
        animationController.SetAbilityState(GetAbilityStateInt());
        //ToggleAnimationState();
    }

    protected void ToggleAnimationState()
    {
        ((PlayerEntityAnimationController)m_animationController).SetAbilityMode(m_abilityMode, GetAbilityStateInt());
    }

    private int GetAbilityStateInt()
    {
        switch (m_abilities.GetActiveAbility())
        {
            case AbilityEnum.None: return 0;
            case AbilityEnum.Shoot: return 1;
            case AbilityEnum.Dash: return 2;
            case AbilityEnum.Block: return 3;
            default: return 0;
        }
    }

    protected void CycleAbilityR(InputAction.CallbackContext context)
    {
        m_abilityDirection = Vector2Int.zero;
        m_abilities.SetActiveAbility(m_abilities.RightAbility());

        animationController.SetAbilityState(GetAbilityStateInt());

        audioModule.PlayAudioEvent("event:/SFX/UI/Vitalsfx_ability_right");

        //SetAbilityAnimationFlag();
    }

    protected void CycleAbilityL(InputAction.CallbackContext context)
    {
        m_abilityDirection = Vector2Int.zero;
        m_abilities.SetActiveAbility(m_abilities.LeftAbility());

        animationController.SetAbilityState(GetAbilityStateInt());

        audioModule.PlayAudioEvent("event:/SFX/UI/Vitalsfx_ability_left");
        //SetAbilityAnimationFlag();
    }

    public void MoveToNewScene()
    {
        StoreHeathEnergy();
        levelModule.SaveGame();

        m_sceneHasSwitched = true;
        levelModule.lvlTransitionInfo = m_currentNode.lvlTransitionInfo;

        // transition to a new scene
        sceneModule.SwitchScene(
            m_currentNode.lvlTransitionInfo.targetSceneName,
            m_currentNode.lvlTransitionInfo.transitionType,
            m_currentNode.lvlTransitionInfo.loadType
            );
    }

    public void LostWoodsTransition()
    {
        StoreHeathEnergy();
        levelModule.SaveGame();

        m_sceneHasSwitched = true;
        // check lost woods count
        if (!levelModule.persistantSceneData._switching)
        {
            levelModule.persistantSceneData._switching = true;

            if (levelModule.persistantSceneData._MisplacedForestCounter == 0 && LastDirection == Vector2Int.down)
            {
                m_currentNode.lvlTransitionInfo.targetNodeIndex = new Vector2Int(2, 7);
                m_currentNode.lvlTransitionInfo.targetSceneName = "Mushroom Forest Start";
                m_currentNode.lvlTransitionInfo.targetRoomIndex = 8;

                levelModule.lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                Destroy(levelModule.persistantSceneData._soundEmitter);
                levelModule.persistantSceneData = new MisplacedForestPersistantSceneData();

                sceneModule.SwitchScene(
                    m_currentNode.lvlTransitionInfo.targetSceneName,
                    m_currentNode.lvlTransitionInfo.transitionType,
                    m_currentNode.lvlTransitionInfo.loadType
                    );
            }
            else if (levelModule.persistantSceneData._MisplacedForestCounter >= 3 && LastDirection == levelModule.persistantSceneData._direction)
            {
                m_currentNode.lvlTransitionInfo.targetNodeIndex = new Vector2Int(0, 3);
                m_currentNode.lvlTransitionInfo.targetSceneName = "Mushroom Forest Start";
                m_currentNode.lvlTransitionInfo.targetRoomIndex = 3;
                levelModule.lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                Destroy(levelModule.persistantSceneData._soundEmitter);
                levelModule.persistantSceneData = new MisplacedForestPersistantSceneData();

                sceneModule.SwitchScene(
                m_currentNode.lvlTransitionInfo.targetSceneName,
                m_currentNode.lvlTransitionInfo.transitionType,
                m_currentNode.lvlTransitionInfo.loadType
                );
            }
            else
            {
                levelModule.lvlTransitionInfo = m_currentNode.lvlTransitionInfo;
                if (LastDirection == levelModule.persistantSceneData._direction)
                {
                    levelModule.persistantSceneData._MisplacedForestCounter++;
                    Debug.Log(levelModule.persistantSceneData._MisplacedForestCounter);
                }
                else
                {
                    levelModule.persistantSceneData._MisplacedForestCounter = 0;
                    Debug.Log(levelModule.persistantSceneData._MisplacedForestCounter);
                }

                sceneModule.SwitchScene(
                m_currentNode.lvlTransitionInfo.targetSceneName,
                m_currentNode.lvlTransitionInfo.transitionType,
                m_currentNode.lvlTransitionInfo.loadType
                );
            }
        }
    }

    public void CheckSceneTransitions()
    {
        if (m_currentNode.overridden)
        {
            if (m_currentNode.overrideType == NodeOverrideType.SceneConnection)
            {
                MoveToNewScene();
            }

            if (m_currentNode.overrideType == NodeOverrideType.LostWoodsConnection)
            {
                LostWoodsTransition();
            }
        }
    }

    public override void EndStep()
    {
        if (m_abilityUsedThisTurn && Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash)
            m_flags.SetFlags(flags.isAirBorn, false);

        base.EndStep();

        if (m_currentNode != m_previousNode) //  && m_previousNode != null)
        {
            audioModule.PlayAudioEvent("event:/SFX/Player/sfx_footstep");
        }

        Abilities.Refresh();

        if (!m_abilityUsedThisTurn)
            Energy++;

        m_abilityUsedThisTurn = false;

        if (Energy > MaxEnergy)
            Energy = MaxEnergy;

        if (Energy < 0)
            Energy = 0;

        CheckSceneTransitions();

        if (m_currentNode != null)
        {
            if (m_currentNode.roomIndex != m_roomIndex)
            {
                m_roomIndex = m_currentNode.roomIndex;
                m_stepController.m_targetRoomIndex = m_roomIndex;
                Debug.Log("we have changed room");
                if (m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name))
                {
                    if (!m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(m_roomIndex))
                        m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Add(m_roomIndex);
                }
                else
                    m_dictRoomsTraveled.Add(SceneManager.GetActiveScene().name, new List<int> { m_roomIndex });

                UpdateSaveFile();
                // we have changed room
            }
        }
    }

    public override void AttackStep()
    {
        if (!m_abilityMode && m_abilities.GetActiveAbility() == AbilityEnum.Shoot && m_abilityDirection != Vector2Int.zero)
        {
            Shoot();
            m_abilityDirection = Vector2Int.zero;
        }
    }

    public bool MoveToRespawnLocation()
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
                ExecuteStep(); // jank solution but it works
                // App.CameraController.MoveToRoomByIndex(respawnPoint.roomIndex);
                return true;
            }
        }
        return false;
    }

    public override void OnDeath()
    {
        GameObject temp = Resources.Load<GameObject>("prefabs/UI prefabs/DeathScreenCanvas");
        Instantiate(temp).transform.parent = Camera.main.transform;
        Health = MaxHealth;
        Energy = MaxEnergy;
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

    public void UpdateInputMode()
    {
        bool hold = levelModule.HoldForAbilityMode;
        AbilityInputMode iMode;
        if (hold)
        {
            iMode = AbilityInputMode.Hold;
        }
        else
        {
            iMode = AbilityInputMode.Toggle;
        }

        if (abilityInputMode != iMode)
        {
            if (iMode == AbilityInputMode.Hold)
                Debug.Log("Player is using \"Hold for Ability Mode\"");
            else if (iMode == AbilityInputMode.Toggle)
                Debug.Log("Player is using \"Toggle for Ability Mode\"");

            abilityInputMode = iMode;
            m_abilityMode = false;
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
                if (LastDirection != Vector2Int.zero)
                {
                    // node = m_previousNode;
                    node = m_currentNode;

                    if (LastDirection == m_abilityDirection)
                    {
                    }
                    else
                    {
                        node = node.GetNeighbour(-LastDirection);
                    }
                }
                else
                {
                    // player didn't move
                    node = m_currentNode;
                }
                SetAbilityAnimationFlag(0);
                animationController.StopLuvParticles();

                m_abilityUsedThisTurn = true;
                GameObject bullet;

                if (SpawnBullet(out bullet, m_bulletPrefab, node, m_abilityDirection))
                {
                    animationController.MuzzleFlash(m_abilityDirection);
                    audioModule.PlayAudioEvent("event:/SFX/Player/sfx_shoot");

                    Energy -= m_shootEnergyCost;
                    if (bullet != null)
                        animationController.SetBulletColour(bullet);

                    return;
                }
                else
                {
                    audioModule.PlayAudioEvent("event:/SFX/Player/sfx_ability_fail");
                    return;
                }
            }
        }
        else
        {
            audioModule.PlayAudioEvent("event:/SFX/Player/sfx_ability_fail");
            return;
        }
    }

    private bool Dash()
    {
        if (Energy >= m_dashEnergyCost)
        {
            if (m_abilityDirection != Vector2.zero)
            {
                m_abilityUsedThisTurn = true;
                Energy -= m_dashEnergyCost;
                m_flags.SetFlags(flags.isAirBorn, true);
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
                m_abilityUsedThisTurn = true;

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

    private void SetInteractPosition()
    {
        Vector3 size = GetComponent<BoxCollider2D>().bounds.extents;
        Vector3 offset = size + transform.GetChild(0).GetChild(0).position;
        m_interactToolTip.transform.position = offset;
    }

    public void StoreHeathEnergy()
    {
        levelModule.ActiveSaveData.currentHealth = Health;
        levelModule.ActiveSaveData.currentEnergy = Energy;
    }

    public void StoreRespawnLoaction()
    {
        levelModule.ActiveSaveData.useRespawnData = true;
        levelModule.ActiveSaveData.respawnRoomID = RoomIndex;
        levelModule.ActiveSaveData.respawnLocation = Position.grid;
        levelModule.ActiveSaveData.levelId = LevelModule.CurrentLevelId();
    }

    public override void OnHit(int damage, float offsetTime = 0f)
    {
        base.OnHit(damage);

        audioModule.PlayAudioEvent("event:/SFX/Player/sfx_player_hit");

        if (isDead)
        {
            animationController.DamageFlash();
            App.CameraController.CameraShake(9f, 0.2f);
        }
        else
            StartCoroutine(OnHitFX(offsetTime));
    }

    private System.Collections.IEnumerator OnHitFX(float time)
    {
        yield return new WaitForSeconds(time);

        animationController.DamageFlash();

        animationController.CameraShake(9f, 0.2f);
    }

    public override void ResolveMoveStep()
    {
        if(m_currentNode != null)
        {
            foreach(var e in m_currentNode.GetGridEntities())
            {
                if(e.GetType() == typeof(WyrmHead) || e.GetType() == typeof(WyrmBody))
                {
                    OnHit(1);
                }
            }
        }

        base.ResolveMoveStep();


    }
}