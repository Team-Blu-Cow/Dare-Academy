using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StepController
{
    // MEMBERS ************************************************************************************
    [SerializeField] private List<GridEntity> m_entities;

    [SerializeField] public int m_currentRoomIndex;
    [SerializeField] public int m_targetRoomIndex;
    private float m_stepTime = 0;
    private float m_timer = 0;
    private TelegraphDrawer m_telegraphDrawer;

    public float stepTime { get { return m_stepTime; } set { m_stepTime = value; } }
    public float timer { get { return m_timer; } set { m_timer = value; } }

    // EVENTS *************************************************************************************
    public delegate void RoomChangedDelegate();

    public event RoomChangedDelegate RoomChangeEvent;

    // INITIALISATION METHODS *********************************************************************
    public StepController()
    {
        m_stepTime = 1;
        m_entities = new List<GridEntity>();
    }

    public StepController(float stepTime, TelegraphDrawer telegraphDrawer)
    {
        m_stepTime = stepTime;
        m_entities = new List<GridEntity>();
        m_telegraphDrawer = telegraphDrawer;
    }

    // STEP METHODS *******************************************************************************
    public bool ExecuteStep()
    {
        if (m_timer < m_stepTime)
            return false;

        m_timer = 0;

        if (m_telegraphDrawer != null)
            m_telegraphDrawer.OnStep();

        for (int i = m_entities.Count - 1; i >= 0; i--)
        {
            if (m_entities[i] == null)
                m_entities.RemoveAt(i);
        }

        ResetAnimations();

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

        CheckForRoomChange();

        AnalyseStep();

        return true;
    }

    // STEP STAGES ********************************************************************************

    public void ResetAnimations()
    {
        for (int j = m_entities.Count - 1; j >= 0; j--)
        {
            m_entities[j].ResetAnimations();
        }
    }

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

    public void CheckForRoomChange()
    {
        if (m_currentRoomIndex != m_targetRoomIndex)
        {
            m_currentRoomIndex = m_targetRoomIndex;

            RoomChangeEvent?.Invoke();
        }
    }

    public void ForceRoomChangeEvent()
    {
        m_currentRoomIndex = m_targetRoomIndex;
        RoomChangeEvent?.Invoke();
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