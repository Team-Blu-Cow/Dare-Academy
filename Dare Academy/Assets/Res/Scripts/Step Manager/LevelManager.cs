using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using JUtil;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float m_stepTime = 0.1f;
    [SerializeField] private StepController m_stepController;

    [SerializeField] private PathfindingMultiGrid m_grid = null;

    public StepController StepController
    { get { return m_stepController; } }

    public PathfindingMultiGrid Grid
    { set { m_grid = value; } get { return m_grid; } }

    private void Awake()
    {
        m_stepController = new StepController(m_stepTime);
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        m_stepController.InitialAnalyse();
    }

    private void Start()
    {
        StartCoroutine(LateStart());
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

        if (Keyboard.current.spaceKey.isPressed)
        {
            m_stepController.ExecuteStep();
        }
    }
}