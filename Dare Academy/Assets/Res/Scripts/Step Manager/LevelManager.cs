using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using JUtil;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float m_stepTime = 0.2f;
    [SerializeField] public int m_defaultPlayerSpawnIndex;
    [SerializeField] public Vector2Int m_defaultPlayerPosition;

    [SerializeField] private StepController m_stepController;

    [SerializeField] private TelegraphDrawer m_telegraphDrawer;

    [SerializeField] private PathfindingMultiGrid m_grid = null;

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
    }

    private void OnDisable()
    {
        PlayerControls input = blu.App.GetModule<blu.InputModule>().PlayerController;
        input.Player.ExecuteStep.performed -= Step;
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
}