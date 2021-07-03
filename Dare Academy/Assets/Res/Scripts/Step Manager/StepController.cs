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

        do
        {
            PreMoveStep();
            MoveStep();
            ResolvePassThroughStep();
            ResolveMoveStep();
            PostMoveStep();
        }
        while (!AllEntitiesFinishedMoving());

        AttackStep();
        DamageStep();
        EndStep();
        DrawStep();
        AnalyseStep();
    }

    public void InitialAnalyse()
    {
        foreach (GridEntity entity in m_entities)
        {
            if (entity.RoomIndex == m_currentRoomIndex)
                entity.AnalyseStep();
        }
    }

    // STEP STAGES ********************************************************************************

    public void PreMoveStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].PreMoveStep();
        }
    }

    public void MoveStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            if (!m_entities[j].isFinishedMoving && !m_entities[j].isDead)
                m_entities[j].MoveStep();
        }
    }

    public void ResolvePassThroughStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            if (!m_entities[j].isDead)
                m_entities[j].ResolvePassThroughStep();
        }
    }

    public void ResolveMoveStep()
    {
        int counter = 0;
        while (CheckForConflicts())
        {
            if (counter++ > 10)
            {
                Debug.LogWarning("[Step Controller] - conflict could not be resolved");
                break; // something could not be resolved
            }

            for (int j = m_entities.Count - 1; j >= 0; j--)
            {
                if (!m_entities[j].isDead)
                    m_entities[j].ResolveMoveStep();
            }
        }
    }

    public void PostMoveStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].PostMoveStep();
        }
    }

    public void AttackStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].AttackStep();
        }
    }

    public void DamageStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].DamageStep();
        }
    }

    public void EndStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].EndStep();
        }
    }

    public void DrawStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].DrawStep();
        }
    }

    public void AnalyseStep()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].AnalyseStep();
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
            if (m_entities[j].CheckForConflict())
            {
                return true;
            }
        }
        return false;
    }

    protected bool AllEntitiesFinishedMoving()
    {
        foreach (GridEntity entity in m_entities)
        {
            if (!entity.isFinishedMoving)
                return false;
        }
        return true;
    }
}