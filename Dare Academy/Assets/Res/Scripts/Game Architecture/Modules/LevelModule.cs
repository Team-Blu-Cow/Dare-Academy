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
        _default = 0, // default value, open first level?
    }

    public class LevelModule : Module
    {
        private PathfindingMultiGrid m_grid = null;
        private GameEventFlags m_gameEventFlags = new GameEventFlags();

        public SaveData ActiveSaveSata
        {
            get { return blu.App.GetModule<IOModule>().savedata; }
            set { blu.App.GetModule<IOModule>().savedata = value; }
        }

        public bool IsSaveLoaded
        {
            get => ActiveSaveSata != null;
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

        private LevelTransitionInformation m_lvlTransitionInfo;

        public LevelTransitionInformation lvlTransitionInfo
        { get { return m_lvlTransitionInfo; } set { m_lvlTransitionInfo = value; } }

        private PersistantSceneData m_persistantSceneData = new PersistantSceneData();

        public PersistantSceneData persistantSceneData
        {
            get { return m_persistantSceneData; }

            set { m_persistantSceneData = value; }
        }

        private GameObject m_playerPrefab;

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= LevelChanged;
        }

        public async override void Initialize()
        {
            SceneManager.sceneLoaded += LevelChanged;

            IOModule ioModule = App.GetModule<IOModule>();

            await ioModule.awaitInitialised;

            if (!ioModule.isSaveLoaded)
            {
                Debug.LogWarning("[Level Module] save file not loaded, creating new save");
                await ioModule.CreateNewSave("new save", true);
            }

            m_gameEventFlags._FlagData = ActiveSaveSata.gameEventFlags;

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
            //m_levelManager.StepController.InitialAnalyse();
        }

        public void AddEntityToCurrentRoom(GridEntity entity)
        {
            m_levelManager.AddEntityToStepController(entity);
        }

        // FILE IO

        public async void SaveGame()
        {
            App.GetModule<IOModule>().savedata.gameEventFlags = m_gameEventFlags._FlagData;
            // TODO @matthew - move the await out of here
            await App.GetModule<IOModule>().SaveAsync();
        }

        public Task<bool> AwaitSaveLoad()
        {
            return Task.Run(() => AwaitSaveLoadImpl());
        }

        internal bool AwaitSaveLoadImpl()
        {
            blu.LevelModule levelModule = blu.App.GetModule<blu.LevelModule>();
            while (levelModule.IsSaveLoaded == false)
            { }
            return true;
        }
    }

    public class PersistantSceneData
    {
        public int _MisplacedForestCounter = 0;
        public Vector2Int _direction = Vector2Int.zero;
        public GameObject _soundEmitter = null;
        public bool _switching = false;
    }
}