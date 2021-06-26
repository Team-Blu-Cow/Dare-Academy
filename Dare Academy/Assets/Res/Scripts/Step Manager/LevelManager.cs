using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil.Grids;
using JUtil;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [SerializeField] StepController stepController;

    [SerializeField] private PathfindingMultiGrid<GridNode> grid = null;

    public StepController StepController
    { get { return stepController; } }
    public PathfindingMultiGrid<GridNode> Grid
    { set { grid = value; } get { return grid; } }

    float stepTimer = 0;

    private void Awake()
    {
        stepController = new StepController();
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        stepController.InitialAnalyse();
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    private void OnDrawGizmos()
    {
        grid.DrawGizmos();
    }

    public void AddEntityToStepController(GridEntity entity)
    {
        stepController.AddEntity(entity);
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            stepController.ExecuteStep();
            stepTimer = 0;
        }

        stepTimer += Time.deltaTime;

        if (!stepController.canStepAgain && stepTimer > stepController.stepTime)
            stepController.canStepAgain = true;
    }

}
