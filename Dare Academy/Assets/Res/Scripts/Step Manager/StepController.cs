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

    public const int NUMBER_OF_STEPS = 8;

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

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].MoveStep();
        }

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].ResolvePassThrough();
        }

        
        int counter = 0;
        while (CheckForConflicts())
        {
            if(counter++ > 10)
            {
                Debug.LogWarning("[Step Controller] - conflict could not be resolved");
                break; // something could not be resolved
            }

            for (int j = m_entities.Count - 1; j >= 0; j--)
            {
                m_entities[j].ResolveMoveStep();
            }
        } 


        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].AttackStep();
        }

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].DamageStep();
        }

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].EndStep();
        }

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].DrawStep();
        }

        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].AnalyseStep();
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

    protected bool CheckForConflicts()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            if(m_entities[j].CheckForConflict())
            {
                return true;
            }
        }
        return false;
    }
}