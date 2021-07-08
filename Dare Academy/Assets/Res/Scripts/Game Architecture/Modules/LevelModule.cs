using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using UnityEngine.SceneManagement;

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

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= LevelChanged;
        }

        public async override void Initialize()
        {
            SceneManager.sceneLoaded += LevelChanged;
            m_grid = null;

            IOModule ioModule = App.GetModule<IOModule>();

            await ioModule.awaitInitialised;

            if (!ioModule.isSaveLoaded)
            {
                Debug.LogWarning("[Level Module] save file not loaded, creating new save");
                await ioModule.CreateNewSave("new save", true);
            }

            m_gameEventFlags.FlagData = ActiveSaveSata.gameEventFlags;
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

            m_gameEventFlags.FlagData = ActiveSaveSata.gameEventFlags;

            m_grid.Initialise();
            //m_levelManager.StepController.InitialAnalyse();
        }

        public void AddEntityToCurrentRoom(GridEntity entity)
        {
            m_levelManager.AddEntityToStepController(entity);
        }

        // FILE IO

        public async void SaveGame()
        {
            App.GetModule<IOModule>().savedata.gameEventFlags = m_gameEventFlags.FlagData;
            // TODO @matthew - move the await out of here
            await App.GetModule<IOModule>().SaveAsync();
        }
    }
}