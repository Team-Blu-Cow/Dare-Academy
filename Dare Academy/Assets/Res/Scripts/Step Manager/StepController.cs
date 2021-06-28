using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StepController
{
    // MEMBERS ************************************************************************************
    [SerializeField] private List<GridEntity> m_entities;

    [SerializeField] public int m_currentRoomIndex;
    [SerializeField] public float m_stepTime;

    [HideInInspector] public bool m_canStepAgain;

    public const int NUMBER_OF_STEPS = 7;

    // INITIALISATION METHODS *********************************************************************
    public StepController()
    {
        m_entities = new List<GridEntity>();
    }

    // STEP METHODS *******************************************************************************
    public void ExecuteStep()
    {
        if (!m_canStepAgain)
            return;

        m_canStepAgain = false;

        for (int i = 0; i < NUMBER_OF_STEPS; i++)
        {
            for (int j = m_entities.Count - 1; j >= 0; j--)
            {
                switch (i)
                {
                    case 0:
                        m_entities[j].MoveStep();
                        break;

                    case 1:
                        m_entities[j].ResolveMoveStep();
                        break;

                    case 2:
                        m_entities[j].AttackStep();
                        break;

                    case 3:
                        m_entities[j].DamageStep();
                        break;

                    case 4:
                        m_entities[j].EndStep();
                        break;

                    case 5:
                        m_entities[j].DrawStep();
                        break;

                    case 6:
                        m_entities[j].AnalyseStep();
                        break;
                }
            }
        }
    }

    public void InitialAnalyse()
    {
        foreach (GridEntity entity in m_entities)
        {
            if (entity.RoomIndex == m_currentRoomIndex)
                entity.AnalyseStep();
        }
    }

    // HELPER METHODS *****************************************************************************
    public void AddEntity(GridEntity entity)
    {
        if (m_entities.Contains(entity))
            return;
        m_entities.Add(entity);
    }

    // returns false if object was not in the list
    public bool RemoveEntity(GridEntity entity)
    {
        return m_entities.Remove(entity);
    }
}