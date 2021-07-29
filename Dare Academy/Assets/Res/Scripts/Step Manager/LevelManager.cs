using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using JUtil;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using blu;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance => _instance;

    private int m_forceStepsCount = 0;
    private double m_forceStepTime = 0d;

    private double m_forceStepTimer = 0d;

    public int ForceStepsCount
    {
        get => m_forceStepsCount;
        set
        {
            m_forceStepsCount = value;
            m_forceStepTimer = 0d;
        }
    }

    public double ForceStepTime
    {
        get => m_forceStepTime;
        set => m_forceStepTime = value;
    }

    public bool AllowPlayerMovement
    {
        get { return m_forceStepsCount == 0; }
    }

    public bool debug_SpawnPlayer = true;
    [SerializeField] private float m_stepTime = 0.2f;
    [SerializeField] public int m_defaultPlayerSpawnIndex;
    [SerializeField] public Vector2Int m_defaultPlayerPosition;

    [SerializeField] private StepController m_stepController;

    [SerializeField] private TelegraphDrawer m_telegraphDrawer;

    [SerializeField] private PathfindingMultiGrid m_grid = null;
    private bool paused = false;

    public StepController StepController
    { get { return m_stepController; } }

    public TelegraphDrawer TelegraphDrawer
    { get { return m_telegraphDrawer; } }

    public PathfindingMultiGrid Grid
    { set { m_grid = value; } get { return m_grid; } }

    private void Awake()
    {
        _instance = this;
        m_telegraphDrawer.Initialise();
        m_stepController = new StepController(m_stepTime, m_telegraphDrawer);
    }

    private void OnValidate()
    {
        m_telegraphDrawer.OnValidate();
    }

    private void OnEnable()
    {
        PlayerControls input = blu.App.GetModule<blu.InputModule>().PlayerController;
        input.Player.ExecuteStep.performed += Step;
        App.GetModule<InputModule>().SystemController.UI.Pause.performed += PauseGame;
        App.GetModule<InputModule>().SystemController.UI.Back.performed += BackPause;
    }

    private void OnDisable()
    {
        PlayerControls input = blu.App.GetModule<blu.InputModule>().PlayerController;
        input.Player.ExecuteStep.performed -= Step;
        App.GetModule<InputModule>().SystemController.UI.Pause.performed -= PauseGame;
        App.GetModule<InputModule>().SystemController.UI.Back.performed -= BackPause;
    }

    private void OnDrawGizmos()
    {
        m_grid.DrawGizmos();
    }

    public void AddEntityToStepController(GridEntity entity)
    {
        m_stepController.AddEntity(entity);
    }

    private void Update()
    {
        StepController.timer += Time.deltaTime;
        if (ForceStepsCount > 0)
        {
            m_forceStepTimer += Time.deltaTime;
            if (m_forceStepTimer > m_forceStepTime)
            {
                m_forceStepTimer = 0;
                if (m_stepController.ExecuteStep())
                {
                    ForceStepsCount--;
                }
            }
        }
    }

    private void Step(InputAction.CallbackContext context)
    {
        if (AllowPlayerMovement)
            m_stepController.ExecuteStep();
    }

    public void PauseGame(InputAction.CallbackContext ctx)
    {
        if (paused)
        {
            if (App.CanvasManager.topCanvas.name != "Options Menu")
                App.CanvasManager.CloseCanvas();
            App.CanvasManager.CloseCanvas();

            EventSystem.current.SetSelectedGameObject(null);

            App.GetModule<InputModule>().PlayerController.Player.Enable();
            App.GetModule<InputModule>().SystemController.UI.Map.Enable();

            if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 0);

            paused = false;
        }
        else
        {
            App.CanvasManager.OpenCanvas("Options Menu", true);
            EventSystem.current.SetSelectedGameObject(App.CanvasManager.GetCanvasContainer("Options Menu").gameObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject);

            if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 1);

            App.GetModule<InputModule>().PlayerController.Player.Disable();
            App.GetModule<InputModule>().SystemController.UI.Map.Disable();

            CanvasTool.CanvasContainer mapCanvas = App.CanvasManager.GetCanvasContainer("Map");
            if (App.CanvasManager.openCanvases.Contains(mapCanvas))
            {
                mapCanvas.CloseCanvas();
                mapCanvas.gameObject.GetComponentInChildren<MiniMapGen>().Open = false;

                if (mapCanvas.gameObject.transform.GetChild(2).TryGetComponent(out QuestLog questLog))
                    questLog.Open = false;

                App.CanvasManager.openCanvases.Remove(mapCanvas);
            }
            paused = true;
        }
    }

    public void BackPause(InputAction.CallbackContext ctx)
    {
        if (paused)
        {
            if (App.CanvasManager.topCanvas.name == "Options Menu")
            {
                paused = false;
                App.GetModule<InputModule>().PlayerController.Player.Enable();
                App.GetModule<InputModule>().SystemController.UI.Map.Enable();

                if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                    App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 0);

                EventSystem.current.SetSelectedGameObject(null);
            }

            App.CanvasManager.CloseCanvas();
            EventSystem.current.SetSelectedGameObject(App.CanvasManager.GetCanvasContainer("Options Menu").gameObject.transform.GetChild(1).GetChild(0).GetChild(2).gameObject);
        }
    }

    public void Pause()
    {
        PauseGame(new InputAction.CallbackContext());
    }

    public void SaveGame()
    {
        App.GetModule<LevelModule>().SaveGame();
    }
}