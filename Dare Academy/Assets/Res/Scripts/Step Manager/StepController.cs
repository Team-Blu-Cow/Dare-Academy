using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StepController
{
    // MEMBERS ************************************************************************************
    [SerializeField] List<GridEntity> entities;
    [SerializeField] public int currentRoomIndex;
    [SerializeField] public float stepTime;

    [HideInInspector] public bool canStepAgain;

    public const int NUMBER_OF_STEPS = 7;

    // INITIALISATION METHODS *********************************************************************
    public StepController()
    {
        entities = new List<GridEntity>();
    }

    // STEP METHODS *******************************************************************************
    public void ExecuteStep()
    {
        if (!canStepAgain)
            return;

        canStepAgain = false;

        for (int i = 0; i < NUMBER_OF_STEPS; i++)
        {
            foreach(GridEntity entity in entities)
            {
                switch(i)
                {
                    case 0:
                        entity.MoveStep();
                        break;
                    case 1:
                        entity.ResolveMoveStep();
                        break;
                    case 2:
                        entity.AttackStep();
                        break;
                    case 3:
                        entity.DamageStep();
                        break;
                    case 4:
                        entity.EndStep();
                        break;
                    case 5:
                        entity.DrawStep();
                        break;
                    case 6:
                        entity.AnalyseStep();
                        break;

                }
            }
        }
    }

    public void InitialAnalyse()
    {
        foreach (GridEntity entity in entities)
        {
            entity.AnalyseStep();
        }
    }

    // HELPER METHODS *****************************************************************************
    public void AddEntity(GridEntity entity)
    {
        if (entities.Contains(entity))
            return;
        entities.Add(entity);
    }
}
