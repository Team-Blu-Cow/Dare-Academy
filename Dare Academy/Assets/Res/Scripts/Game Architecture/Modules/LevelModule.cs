using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using UnityEngine.SceneManagement;

namespace blu
{

    public class LevelModule : Module
    {
        private PathfindingMultiGrid<GridNode> grid = null;
        public PathfindingMultiGrid<GridNode> MetaGrid
        { set { grid = value; } get { return grid; } }

        public Grid<GridNode> Grid(int i)
        { return grid.Grid(i); }

        private LevelManager levelManager = null;
        public LevelManager LevelManager
        { get { return levelManager; } }
        public StepController StepController
        { get { return levelManager.StepController; } }


        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= LevelChanged;
        }

        public override void Initialize()
        {
            SceneManager.sceneLoaded += LevelChanged;
            grid = null;
        }

        protected override void SetDependancies()
        {
            grid = null;
        }

        public void LevelChanged(Scene scene, LoadSceneMode loadSceneMode)
        {
            levelManager = null;
            levelManager = FindObjectOfType<LevelManager>();

            if (levelManager == null)
                return;

            grid = levelManager.Grid;

            grid.Initialise();
            levelManager.StepController.InitialAnalyse();
        }

        public void AddEntityToCurrentRoom(GridEntity entity)
        {
            levelManager.AddEntityToStepController(entity);
        }
    }

}