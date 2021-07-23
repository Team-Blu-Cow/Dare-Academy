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
        App.GetModule<AudioModule>().PlayMusicEvent("event:/Music/Crash Site/Crash Site"); //#todo remove
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
    }

    private void Step(InputAction.CallbackContext context)
    {
        m_stepController.ExecuteStep();
    }

    public void PauseGame(InputAction.CallbackContext ctx)
    {
        if (paused)
        {
            if (App.CanvasManager.topCanvas.name != "Options Menu")
                App.CanvasManager.CloseCanvas();
            App.CanvasManager.CloseCanvas();

            App.GetModule<InputModule>().PlayerController.Player.Enable();
            if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 0);

            paused = false;
        }
        else
        {
            App.CanvasManager.OpenCanvas("Options Menu", true);
            EventSystem.current.SetSelectedGameObject(App.CanvasManager.GetCanvasContainer("Options Menu").gameObject.transform.GetChild(1).GetChild(1).gameObject);
            if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 1);
            App.GetModule<InputModule>().PlayerController.Player.Disable();
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
                if (App.GetModule<AudioModule>().GetCurrentSong() != null)
                    App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", 0);
            }

            App.CanvasManager.CloseCanvas();
            EventSystem.current.SetSelectedGameObject(App.CanvasManager.GetCanvasContainer("Options Menu").gameObject.transform.GetChild(1).GetChild(1).gameObject);
        }
    }

    public void Pause()
    {
        PauseGame(new InputAction.CallbackContext());
    }
}