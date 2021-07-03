using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using JUtil;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float m_stepTime = 0.1f;
    [SerializeField] StepController m_stepController;

    [SerializeField] private PathfindingMultiGrid<GridNode> m_grid = null;

    public StepController StepController
    { get { return m_stepController; } }
    public PathfindingMultiGrid<GridNode> Grid
    { set { m_grid = value; } get { return m_grid; } }

    [SerializeField] float m_stepTimer = 0;

    private void Awake()
    {
        m_stepController = new StepController(m_stepTime);
    }

    IEnumerator LateStart()
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
        if(m_stepController.m_canStepAgain && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            m_stepController.ExecuteStep();
            m_stepTimer = 0;
            m_stepController.m_canStepAgain = false;
        }

        m_stepTimer += Time.deltaTime;

        if (!m_stepController.m_canStepAgain && m_stepTimer > m_stepController.m_stepTime)
            m_stepController.m_canStepAgain = true;
    }

}
