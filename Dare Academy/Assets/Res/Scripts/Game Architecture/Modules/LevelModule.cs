using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace blu
{
    // for use when saving game to signify which scene should be loaded when opening the game
    public enum LevelID
    {
        _default = 0,
        crashsite_top = 1,
        crashsite_bottom = 2,
        mushroom_start = 3,
        mushroom_end = 4,
        misplaced_forest = 5,
    }

    public class LevelModule : Module
    {
        private PathfindingMultiGrid m_grid = null;
        private GameEventFlags m_gameEventFlags = new GameEventFlags();

        public bool Initialised
        { get; private set; }

        public SaveData ActiveSaveData
        {
            get { return blu.App.GetModule<IOModule>().ActiveSaveData; }
            set { blu.App.GetModule<IOModule>().ActiveSaveData = value; }
        }

        public bool IsSaveLoaded
        {
            get => ActiveSaveData != null;
        }

        public GameEventFlags EventFlags
        {
            get { return m_gameEventFlags; }
            set { m_gameEventFlags = value; }
        }

        public PathfindingMultiGrid MetaGrid
        { set { m_grid = value; } get { return m_grid; } }

        public Grid<GridNode> CurrentRoom { get => Grid(StepController.m_currentRoomIndex); }

        public Grid<GridNode> Grid(int i)
        { return m_grid.Grid(i); }

        private LevelManager m_levelManager = null;

        public LevelManager LevelManager
        { get { return m_levelManager; } }

        public StepController StepController
        { get { return m_levelManager.StepController; } }

        public TelegraphDrawer telegraphDrawer
        { get { return m_levelManager.TelegraphDrawer; } }

        private LevelTransitionInformation m_lvlTransitionInfo;

        public LevelTransitionInformation lvlTransitionInfo
        { get { return m_lvlTransitionInfo; } set { m_lvlTransitionInfo = value; } }

        private MisplacedForestPersistantSceneData m_persistantSceneData = new MisplacedForestPersistantSceneData();

        public MisplacedForestPersistantSceneData persistantSceneData
        {
            get { return m_persistantSceneData; }

            set { m_persistantSceneData = value; }
        }

        private GameObject m_playerPrefab;

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= LevelChanged;
            SaveGame();
        }

        public override void Initialize()
        {
            SceneManager.sceneLoaded += LevelChanged;

            if (m_playerPrefab == null)
                m_playerPrefab = Resources.Load<GameObject>("prefabs/Entities/Player");
        }

        protected override void SetDependancies()
        {
            _dependancies.Add(typeof(IOModule));
        }

        public void LevelChanged(Scene scene, LoadSceneMode loadSceneMode)
        {
            m_levelManager = null;
            m_levelManager = FindObjectOfType<LevelManager>();

            if (m_levelManager == null)
                return;

            m_grid = m_levelManager.Grid;

            RespawnStationEntity.CurrentRespawnStation = null;
            //m_gameEventFlags.FlagData = ActiveSaveSata.gameEventFlags;

            m_grid.Initialise();

            if (m_playerPrefab == null)
                m_playerPrefab = Resources.Load<GameObject>("prefabs/Entities/Player");

            if (m_levelManager.debug_SpawnPlayer == true)
                SpawnPlayer();
            else
                FindPlayer();

            //m_levelManager.StepController.InitialAnalyse();
        }

        public void FindPlayer()
        {
            PlayerEntity playerEnt = FindObjectOfType<PlayerEntity>();
            if (playerEnt == null)
                SpawnPlayer();

            playerEnt.DebugSetNode();

            m_levelManager.StepController.m_currentRoomIndex = playerEnt.RoomIndex;
            m_levelManager.StepController.m_targetRoomIndex = playerEnt.RoomIndex;
        }

        public void SpawnPlayer()
        {
            if (m_levelManager.debug_SpawnPlayer == true)

                if (m_lvlTransitionInfo == null)
                {
                    Vector3 pos = m_grid.Grid(m_levelManager.m_defaultPlayerSpawnIndex)[m_levelManager.m_defaultPlayerPosition].position.world;

                    m_levelManager.StepController.m_currentRoomIndex = m_levelManager.m_defaultPlayerSpawnIndex;
                    m_levelManager.StepController.m_targetRoomIndex = m_levelManager.m_defaultPlayerSpawnIndex;

                    Instantiate(m_playerPrefab, pos, Quaternion.identity);
                }
                else
                {
                    Vector2Int nodePos = m_lvlTransitionInfo.targetNodeIndex + ((-m_lvlTransitionInfo.offsetVector) * m_lvlTransitionInfo.offsetIndex);

                    GridNode node = m_grid.Grid(m_lvlTransitionInfo.targetRoomIndex)[nodePos];

                    if (node == null)
                    {
                        node = m_grid.Grid(m_lvlTransitionInfo.targetRoomIndex)[m_lvlTransitionInfo.targetNodeIndex];
                    }

                    Vector3 pos;

                    if (node == null)
                    {
                        pos = m_grid.Grid(m_levelManager.m_defaultPlayerSpawnIndex)[m_levelManager.m_defaultPlayerPosition].position.world;
                        m_levelManager.StepController.m_currentRoomIndex = m_levelManager.m_defaultPlayerSpawnIndex;
                        m_levelManager.StepController.m_targetRoomIndex = m_levelManager.m_defaultPlayerSpawnIndex;
                    }
                    else
                    {
                        pos = node.position.world;
                        m_levelManager.StepController.m_currentRoomIndex = m_lvlTransitionInfo.targetRoomIndex;
                        m_levelManager.StepController.m_targetRoomIndex = m_lvlTransitionInfo.targetRoomIndex;
                    }

                    Instantiate(m_playerPrefab, pos, Quaternion.identity);
                }
        }

        public void AddEntityToCurrentRoom(GridEntity entity)
        {
            m_levelManager.AddEntityToStepController(entity);
        }

        // FILE IO

        public void LoadFromSave()
        {
            IOModule io = App.GetModule<IOModule>();

            if (!io.IsSaveLoading && !io.IsSaveLoaded)
            {
                bool found = false;
                for (int i = 0; i < io.SaveSlots.Length; i++)
                {
                    if (io.SaveSlots[i] != null)
                    {
                        found = true;
                        io.LoadSaveAsync(io.SaveSlots[i]);
                        break;
                    }
                }

                if (!found)
                {
                    io.CreateNewSave(0, true);
                }
            }

            while (ActiveSaveData == null)
            {
            }

            EventFlags._FlagData = ActiveSaveData.gameEventFlags;

            App.GetModule<QuestModule>().LoadFromFile();

            Initialised = true;
        }

        public void FlushSaveData()
        {
            Initialised = false;
            EventFlags._FlagData = 0;
        }

        public async void SaveGame()
        {
            await AwaitSaveLoad();
            App.GetModule<QuestModule>().WriteToFile();
            App.GetModule<IOModule>().ActiveSaveData.gameEventFlags = EventFlags._FlagData;
            // #TODO #matthew - move the await out of here
            await App.GetModule<IOModule>().SaveAsync();
        }

        public Task AwaitSaveLoad()
        {
            return Task.Run(() => AwaitSaveLoadImpl());
        }

        internal void AwaitSaveLoadImpl()
        {
            while (!IsSaveLoaded)
            { }

            return;
            // if (!io.IsSaveLoading && !io.IsSaveLoaded)
            // {
            //     bool found = false;
            //     for (int i = 0; i < io.SaveSlots.Length; i++)
            //     {
            //         if (io.SaveSlots[i] != null)
            //         {
            //             found = true;
            //             await io.LoadSaveAsync(io.SaveSlots[i]);
            //             break;
            //         }
            //     }
            //
            //     if (!found)
            //     {
            //         await io.CreateNewSave(0, true);
            //     }
            // }
            //
            // await io.AwaitSaveLoaded();
            //
            // blu.LevelModule levelModule = blu.App.GetModule<blu.LevelModule>();
            // while (levelModule.IsSaveLoaded == false)
            // {
            //     Debug.Log("awaiting saveLoad");
            // }
            // return;
        }

        public Task<bool> AwaitInitialised()
        {
            return Task.Run(() => AwaitInitialisedImpl());
        }

        internal bool AwaitInitialisedImpl()
        {
            while (!Initialised)
            {
            }
            return true;
        }

        // HELPER FUNCTIONS

        public static string ResolveSceneNameString(LevelID id)
        {
            switch (id)
            {
                case LevelID._default:
                    return "Crashsite Top";

                case LevelID.crashsite_top:
                    return "Crashsite Top";

                case LevelID.crashsite_bottom:
                    return "Crashsite Bottom";

                case LevelID.mushroom_start:
                    return "Mushroom Forest Start";

                case LevelID.mushroom_end:
                    return "Mushroom Forest End";

                case LevelID.misplaced_forest:
                    return "Misplaced Forest";

                default:
                    Debug.LogWarning("[LevelModule] could not resolve scene name from LevelId, get @matthew to fix this");
                    return "Crashsite Top";
            }
        }

        public static LevelID ResolveLevelId(int buildIndex)
        {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(buildIndex);

            switch (sceneName)
            {
                case "Assets/Res/Scenes/Game Scenes/Crashsite Top.unity":
                    return LevelID.crashsite_top;

                case "Assets/Res/Scenes/Game Scenes/Crashsite Bottom.unity":
                    return LevelID.crashsite_bottom;

                case "Assets/Res/Scenes/Game Scenes/Mushroom Forest Start.unity":
                    return LevelID.mushroom_start;

                case "Assets/Res/Scenes/Game Scenes/Mushroom Forest End.unity":
                    return LevelID.mushroom_end;

                case "Assets/Res/Scenes/Game Scenes/Misplaced Forest.unity":
                    return LevelID.misplaced_forest;

                default:
                    Debug.LogWarning("[LevelModule] could not resolve scene name from buildIndex, get @matthew to fix this");
                    return LevelID._default;
            }
        }

        public static LevelID CurrentLevelId()
        {
            return ResolveLevelId(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public class MisplacedForestPersistantSceneData
    {
        public int _MisplacedForestCounter = 0;
        public Vector2Int _direction = Vector2Int.zero;
        public GameObject _soundEmitter = null;
        public bool _switching = false;
    }
}