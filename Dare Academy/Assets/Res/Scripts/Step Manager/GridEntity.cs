using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;
using JUtil;
using flags = GridEntityFlags.Flags;

public abstract class GridEntity : MonoBehaviour
{
    // MEMBERS ************************************************************************************

    protected Vector2 m_movementDirection;

    [SerializeField] protected int m_mass = 2;
    protected int m_speed     = 1;
    protected int m_health    = 1;

    [SerializeField] protected int m_roomIndex = 0;

    [SerializeField] protected GridEntityFlags m_flags = new GridEntityFlags();

    public int Mass { get { return m_mass; } set { m_mass = value; } }
    public int Speed { get { return m_speed; } set { m_speed = value; } }
    public int RoomIndex { get { return m_roomIndex; } set { m_roomIndex = value; } }
    public bool isDead { get { return m_health <= 0 && m_flags.IsFlagsSet(flags.isKillable); } }

    public bool RemoveFromList { get { return m_health <= 0; } }

    protected StepController m_stepController;

    protected GridNode m_currentNode = null;
    protected GridNode m_targetNode = null;
    protected GridNode m_previousNode = null;

    public GridNodePosition Position
    { get { return m_currentNode.position; } }

    // INITIALISATION METHODS *********************************************************************
    private void Start()
    {
        m_currentNode = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(transform.position);
        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_roomIndex = m_currentNode.roomIndex;

        transform.position = Position.world;
        m_targetNode = m_currentNode;

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        if (m_roomIndex == m_stepController.m_currentRoomIndex)
        {
            m_stepController.AddEntity(this);
        }
    }

    // STEP FLOW METHODS **************************************************************************
    // step flow: [move] -> [resolve move] -> [attack] -> [damage] -> [end] -> [draw] -> [analyse]

    virtual public void MoveStep()
    {
        // set currentNode to targetNode
        // keep a record of where we came from
        if (m_targetNode != null && m_currentNode != null)
        {
            if (m_previousNode != null)
                m_previousNode.RemoveEntity(this);
            m_currentNode.RemoveEntity(this);
            m_targetNode.RemoveEntity(this);

            m_previousNode = m_currentNode;
            m_currentNode = m_targetNode;
            m_targetNode = null;

            m_currentNode.AddEntity(this);
        }

        Debug.Log("Move Step");
    }

    virtual public void ResolveMoveStep()
    {
        // check for conflict on current node
        if (!CheckForConflict())
            return;

        // check for mass resolution
        if (ResolveMassConflict())
            return;

        // check for speed resolution
        ResolveSpeedConflict();

        // check for player resolution
        ResolvePlayerConflict();

        // resolve randomly
        ResolveRandomConflict();

        // check for any new conflicts
    }

    virtual public void AttackStep()
    {
    }

    virtual public void DamageStep()
    {
    }

    virtual public void EndStep()
    {
        if (isDead)
        {
            // TODO @matthew/@jay - don't remove immediately to allow for death animation
            // kill entity
            m_stepController.RemoveEntity(this);
            GameObject.Destroy(gameObject);
        }
    }

    abstract public void AnalyseStep();

    virtual public void DrawStep()
    {
    }

    // RESOLVE MOVE CONFLICT METHODS **************************************************************
    virtual public bool CheckForConflict()
    {
        // check for conflict on current node
        if (m_currentNode.CheckForConflict())
            return true;

        return CheckForPassThrough();
    }

    virtual public bool CheckForPassThrough()
    {
        // check for any objects that have passed through this entity
        GridNode behindNode = m_currentNode.Neighbors[(-m_movementDirection).RotationToIndex(45)].reference;

        if (behindNode == null)
        {
            // TODO @jay/@matthew : object is being crushed, kill it i guess.
            return false;
        }

        List<GridEntity> entities = behindNode.GetGridEntities();

        if (entities == null || entities.Count < 1)
            return false;

        foreach (GridEntity entity in entities)
        {
            if (entity.Speed > 0 && entity.m_movementDirection == -m_movementDirection)
            {
                return true;
            }
        }

        return false;
    }

    virtual public bool ResolveMassConflict()
    {
        // TODO @jay/@matthew : this does not account for a situation where both a pass-through AND a standard conflict are present

        bool passThrough = CheckForPassThrough();

        List<GridEntity> conflictingEntities;

        if (passThrough)
            conflictingEntities = m_previousNode.GetGridEntities();
        else
            conflictingEntities = m_currentNode.GetGridEntities();

        GridEntity winningEntity = null;

        foreach (var entity in conflictingEntities)
        {
            if (passThrough && (entity.m_movementDirection != -m_movementDirection && entity.Speed > 0))
                continue;

            if (Mass > entity.Mass)
                winningEntity = this;
            else if (Mass < entity.Mass)
                winningEntity = entity;
        }

        if (winningEntity != null)
        {
            // resolve mass conflict
            conflictingEntities.Remove(winningEntity);

            if (winningEntity != this && !conflictingEntities.Contains(this))
                conflictingEntities.Add(this);

            // loop through each "losing" entity and move them back a space
            for (int i = conflictingEntities.Count - 1; i >= 0; i--)
            {
                if (passThrough && (conflictingEntities[i].m_movementDirection != -winningEntity.m_movementDirection && conflictingEntities[i].Speed > 0))
                    continue;
                conflictingEntities[i].MoveBack(winningEntity);
            }

            return true;
        }

        return false;
    }

    virtual public void ResolveSpeedConflict()
    {
    }

    virtual public void ResolvePlayerConflict()
    {
    }

    virtual public void ResolveRandomConflict()
    {
    }

    // HELPER METHODS *****************************************************************************

    public void Update()
    {
        if (RoomIndex == -1)
            return;
        if (m_currentNode != null)
        {
            float xx = Mathf.Lerp(transform.position.x, m_currentNode.position.world.x, 0.5f);
            float yy = Mathf.Lerp(transform.position.y, m_currentNode.position.world.y, 0.5f);
            float zz = Mathf.Lerp(transform.position.z, m_currentNode.position.world.z, 0.5f);
            transform.position = new Vector3(xx, yy, zz);
        }
    }

    public void SetTargetNode(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        m_movementDirection = direction;

        int dir = direction.RotationToIndex(45);
        SetTargetNode(dir);
    }

    virtual public void SetTargetNode(int direction)
    {
        Mathf.Clamp(direction, 0, 7);

        m_previousNode = null;
        m_targetNode = m_currentNode.Neighbors[direction].reference;
    }

    virtual public void MoveBack(GridEntity winningEntity)
    {
        /*if (m_targetNode != null && m_currentNode != null)
        {
            if (m_previousNode != null)
                m_previousNode.RemoveEntity(this);
            m_currentNode.RemoveEntity(this);
            m_targetNode.RemoveEntity(this);

            m_previousNode = m_currentNode;
            m_currentNode = m_targetNode;
            m_targetNode = null;

            m_currentNode.AddEntity(this);
        }*/

        m_currentNode.RemoveEntity(this);

        Vector2 moveDirection = (m_flags.IsFlagsSet(flags.isPushable))? winningEntity.m_movementDirection : -m_movementDirection;

        m_currentNode = winningEntity.m_currentNode.Neighbors[(moveDirection).RotationToIndex(45)].reference;

        if (m_currentNode == null)
        {
            // TODO @jay/@matthew : enemy has been squashed. kill it?
        }

        m_previousNode = null;

        m_currentNode.AddEntity(this);
    }
}