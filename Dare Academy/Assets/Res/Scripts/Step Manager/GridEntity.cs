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

    protected int m_stepsTaken = 0;

    [SerializeField] protected int m_roomIndex = 0;

    [SerializeField] protected GridEntityFlags m_flags = new GridEntityFlags();

    public int Mass { get { return m_mass; } set { m_mass = value; } }
    public int Speed { get { return m_speed; } }
    public int RoomIndex { get { return m_roomIndex; } set { m_roomIndex = value; } }

    public bool isDead
    {
        get
        {
            return
                (m_health <= 0 && m_flags.IsFlagsSet(flags.isKillable))
                || m_currentNode == null
                || m_flags.IsFlagsSet(flags.isDead)
                ; // fuck you Adam, its staying in :] - Love Matthew & Jay
        }
    }

    public bool isFinishedMoving { get { return m_speed <= m_stepsTaken; } }

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
        m_currentNode.AddEntity(this);

        if (m_roomIndex == m_stepController.m_currentRoomIndex)
        {
            m_stepController.AddEntity(this);
        }
    }

    // STEP FLOW METHODS **************************************************************************

    // step flow
    /*
     *         [pre-move]
     *         [move]
     *         [resolve pass through]
     *         [resolve move]
     *         [post-move]
     *         [IF m_stepsTaken != m_speed: GOTO {move}]
     * virtual [attack]
     * virtual [damage]
     *         [end]
     * virtual [draw]
     * virtual [analyse]
     *
     */

    public void PreMoveStep()
    {
        if (m_currentNode == null)
        {
            m_flags.SetFlags(flags.isDead, true);
        }
    }

    public void MoveStep()
    {
        SetTargetNode(m_movementDirection);
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
    }

    public void ResolvePassThroughStep()
    {
        if (!CheckForPassThrough())
            return;

        // TODO @matthew/@jay : this does not respect entities with the isAttack flag yet

        List<GridEntity> winningEntities = new List<GridEntity>(m_previousNode.GetGridEntities());
        List<GridEntity> losingEntities = new List<GridEntity>();
        winningEntities.Add(this);

        int highestMass = int.MinValue;
        int highestSpeed = int.MinValue;

        for (int i = winningEntities.Count - 2; i >= 0; i--)
        {
            if (winningEntities[i].m_movementDirection != -m_movementDirection && winningEntities[i] != this)
            {
                winningEntities.RemoveAt(i);
                continue;
            }

            if (winningEntities[i].Mass != winningEntities[i + 1].Mass)
                highestMass = (winningEntities[i].Mass > winningEntities[i + 1].Mass) ? winningEntities[i].Mass : winningEntities[i + 1].Mass;

            if (winningEntities[i].Speed != winningEntities[i + 1].Speed)
                highestMass = (winningEntities[i].Speed > winningEntities[i + 1].Speed) ? winningEntities[i].Speed : winningEntities[i + 1].Speed;
        }

        if (highestMass > int.MinValue)
        {
            // mass conflict resolution
            ResolveMassConflict(ref winningEntities, ref losingEntities);
            RemovePassThrough(winningEntities, losingEntities);
            return;
        }

        if (highestSpeed > int.MinValue)
        {
            // speed conflict resolution
            ResolveSpeedConflict(ref winningEntities, ref losingEntities);
            RemovePassThrough(winningEntities, losingEntities);
            return;
        }

        foreach (GridEntity entity in winningEntities)
        {
            entity.RemoveFromCurrentNode();

            entity.m_currentNode = entity.m_previousNode;

            entity.AddToCurrentNode();

            entity.m_speed = 0;
            entity.m_movementDirection = Vector2.zero;
        }

        /*
        bool isPlayer = false;

        foreach (var entity in winningEntities)
        {
            if (entity.m_flags.IsFlagsSet(flags.isPlayer))
                isPlayer = true;
        }

        if (isPlayer)
        {
            // player conflict resolution
            ResolvePlayerConflict(ref winningEntities, ref losingEntities);
            RemovePassThrough(winningEntities, losingEntities);
            return;
        }

        // random conflict resolution
        ResolveRandomConflict(ref winningEntities, ref losingEntities);
        RemovePassThrough(winningEntities, losingEntities);//*/
    }

    public void ResolveMoveStep()
    {
        if (m_currentNode == null)
            return;

        List<GridEntity> winning_objects; // everything
        List<GridEntity> losing_objects= new List<GridEntity>();

        // bool passThrough = CheckForPassThrough();
        // if (passThrough)
        //     winning_objects = m_previousNode.GetGridEntities();
        // else
        //     winning_objects = m_currentNode.GetGridEntities();

        winning_objects = new List<GridEntity>(m_currentNode.GetGridEntities());

        // check for conflict on current node
        if (!CheckForConflict())
            return;

        if (winning_objects.Count > 1)
        {
            // check for stationary objects
            ResolveStationaryConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count > 1)
        {
            // check for mass resolution
            ResolveMassConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count > 1)
        {
            // check for speed resolution
            ResolveSpeedConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count > 1)
        {
            // check for player resolution
            ResolvePlayerConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count > 1)
        {
            // resolve randomly
            ResolveRandomConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count == 1)
        { PushBackAll(losing_objects, winning_objects[0]); }

        // check for any new conflicts
    }

    public void PostMoveStep()
    {
        m_stepsTaken++;
    }

    virtual public void AttackStep()
    {
    }

    virtual public void DamageStep()
    {
    }

    public void EndStep()
    {
        m_stepsTaken = 0;
        if (m_currentNode != null && m_currentNode.GetGridEntities().Count > 1) // someone messed up bad, people are inside each other
        {
            List<GridEntity> entities = m_currentNode.GetGridEntities();

            int highestMass = int.MinValue;

            foreach (var entity in entities)
            {
                if (entity.Mass > highestMass)
                {
                    highestMass = entity.Mass;
                }
            }

            if (Mass < highestMass)
            {
                Kill();
            }
        }

        m_speed = 0;

        if (isDead)
        {
            // TODO @matthew/@jay - don't remove immediately to allow for death animation
            // kill entity
            KillSelf();
        }
    }

    virtual public void AnalyseStep()
    {
        return;
    }

    virtual public void DrawStep()
    {
    }

    // RESOLVE MOVE CONFLICT METHODS **************************************************************
    virtual public bool CheckForConflict()
    {
        // check for conflict on current node
        if (m_currentNode != null && m_currentNode.CheckForConflict())
            return true;

        return false;
        //return CheckForPassThrough();
    }

    virtual public bool CheckForPassThrough()
    {
        // get node behind entity
        GridNode behindNode = m_currentNode.Neighbors[(-m_movementDirection).RotationToIndex(45)].reference;

        // pass through is impossible so return
        if (behindNode == null)
            return false;

        // get entities from node
        List<GridEntity> entities = behindNode.GetGridEntities();

        // return if there are no entities
        if (entities == null || entities.Count < 1)
            return false;

        // check for entities that have passed through this entity
        foreach (GridEntity entity in entities)
        {
            if (entity.Speed > 0 && entity.m_movementDirection == -m_movementDirection)
            {
                return true;
            }
        }

        return false;
    }

    virtual protected void ResolveMassConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        // TODO @jay/@matthew : this does not account for a situation where both a pass-through

        int highestMass = int.MinValue;

        foreach (var entity in winning_objects)
        {
            if (entity.Mass > highestMass)
            {
                highestMass = entity.Mass;
            }
        }

        // loop through each "losing" entity and move them back a space
        for (int i = winning_objects.Count - 1; i >= 0; i--)
        {
            if (winning_objects[i].Mass == highestMass)
            {
                continue; // this is the winner, we don't push him you silly billy!
            }

            losing_objects.Add(winning_objects[i]);
            winning_objects.RemoveAt(i);
        }
    }

    virtual protected void ResolveStationaryConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        GridEntity stationary = null;
        bool anyStationary = false;
        bool stationaryIsPushable = false;

        int highestMass = int.MinValue;

        GridEntity top = null;
        GridEntity bottom = null;
        GridEntity left = null;
        GridEntity right = null;

        foreach (GridEntity entity in winning_objects)
        {
            if (entity.m_movementDirection == Vector2.zero)
            {
                stationary = entity;
                anyStationary = true;
                stationaryIsPushable = entity.m_flags.IsFlagsSet(flags.isPushable);

                // break;
            }
            else
            {
                if (entity.m_movementDirection == new Vector2(0, -1))
                {
                    top = entity;
                }
                if (entity.m_movementDirection == new Vector2(1, 0))
                {
                    left = entity;
                }
                if (entity.m_movementDirection == new Vector2(-1, 0))
                {
                    right = entity;
                }
                if (entity.m_movementDirection == new Vector2(0, 1))
                {
                    bottom = entity;
                }
            }
        }

        if (anyStationary)
        {
            if (right != null && left != null)
            {
                winning_objects.Remove(right);
                losing_objects.Add(right);

                winning_objects.Remove(left);
                losing_objects.Add(left);
            }

            if (top != null && bottom != null)
            {
                winning_objects.Remove(top);
                losing_objects.Add(top);

                winning_objects.Remove(bottom);
                losing_objects.Add(bottom);
            }
        }
        foreach (var entity in winning_objects)
        {
            if (entity.Mass > highestMass)
            {
                highestMass = entity.Mass;
            }
        }

        if (anyStationary)
        {
            if (!stationaryIsPushable || stationary.Mass == highestMass)
            {
                for (int i = winning_objects.Count - 1; i >= 0; i--)
                {
                    if (winning_objects[i].m_movementDirection != Vector2.zero)
                    {
                        losing_objects.Add(winning_objects[i]);
                        winning_objects.RemoveAt(i);
                    }
                }
            }
        }
    }

    virtual protected void ResolveSpeedConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        // TODO @jay/@matthew : this does not account for a situation where both a pass-through

        int highestSpeed = int.MinValue;

        foreach (var entity in winning_objects)
        {
            if (entity.Speed > highestSpeed)
            {
                highestSpeed = entity.Speed;
            }
        }

        // loop through each "losing" entity and move them back a space
        for (int i = winning_objects.Count - 1; i >= 0; i--)
        {
            if (winning_objects[i].Speed == highestSpeed)
            {
                continue; // this is the winner, we don't push him you silly billy!
            }

            losing_objects.Add(winning_objects[i]);
            winning_objects.RemoveAt(i);
        }
    }

    virtual protected void ResolvePlayerConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        for (int i = winning_objects.Count - 1; i >= 0; i--)
        {
            if (winning_objects[i].m_flags.IsFlagsSet(flags.isPlayer))
            {
                losing_objects.Add(winning_objects[i]);
                winning_objects.RemoveAt(i);
                break;
            }
        }
    }

    virtual protected void ResolveRandomConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        for (int i = winning_objects.Count - 1; i > 0; i--)
        {
            if (i == 0)
            { continue; }

            losing_objects.Add(winning_objects[i]);
            winning_objects.RemoveAt(i);
        }
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

    public void SetMovementDirection(Vector2 direction, int speed = 1)
    {
        // TODO @matthew/@jay - check this value is valid
        m_movementDirection = direction;
        m_speed = speed;
    }

    protected void SetTargetNode(Vector2 direction, int distance = 1)
    {
        if (direction == Vector2.zero)
            return;

        m_movementDirection = direction;

        int dir = direction.RotationToIndex(45);
        Mathf.Clamp(dir, 0, 7);

        m_previousNode = null;

        m_targetNode = m_currentNode;

        for (int i = 0; i < distance; i++)
        {
            GridNode node = m_targetNode.Neighbors[direction].reference;
            if (node != null)
                m_targetNode = node;
        }

        if (m_targetNode == m_currentNode) // if entity is not moving
        {
            m_targetNode = null;
            m_movementDirection = Vector2.zero;
            m_speed = 0;
        }
    }

    protected static void PushBackAll(List<GridEntity> losers, GridEntity winningEntity)
    {
        foreach (var entity in losers)
        {
            entity.PushBack(winningEntity);
        }
    }

    virtual public void PushBack(GridEntity winningEntity)
    {
        m_currentNode.RemoveEntity(this);

        Vector2 moveDirection;

        // determine move direction
        if (m_flags.IsFlagsSet(flags.isPushable))
        {
            // check to see if entity and winning entity are in the correct position to be pushed by another entity
            // TODO @matthew/@jay : this does not respect entities that have moved with a speed >= 2 due to
            // it using the previous node
            if (winningEntity.m_previousNode.position.grid == m_currentNode.position.grid - winningEntity.m_movementDirection)
                moveDirection = winningEntity.m_movementDirection;
            // if entity should not be pushed, use regular direction
            else
                moveDirection = -m_movementDirection;
        }
        else
        {
            // if both entities are moving the same direction
            if (m_movementDirection == winningEntity.m_movementDirection && m_currentNode.position.grid == winningEntity.Position.grid + winningEntity.m_movementDirection)
                moveDirection = m_movementDirection;
            // normal push logic
            else
            {
                if (m_movementDirection == Vector2.zero)
                {
                    moveDirection = winningEntity.m_movementDirection;
                }
                else
                {
                    moveDirection = -m_movementDirection;
                }
            }
        }

        GridNode lastNode = m_currentNode;
        GridNode winnerLastNode = winningEntity.m_currentNode.Neighbors[(-winningEntity.m_movementDirection).RotationToIndex()].reference;

        m_currentNode = winningEntity.m_currentNode.Neighbors[(moveDirection).RotationToIndex(45)].reference;

        if (m_currentNode == null)
        {
            if (m_flags.IsFlagsSet(flags.isSolid))
            {
                winningEntity.RemoveFromCurrentNode();

                m_currentNode = lastNode;
                winningEntity.m_currentNode = winnerLastNode;

                winningEntity.AddToCurrentNode();
            }
            else
            {
                return;
            }
        }

        m_previousNode = null;

        m_currentNode.AddEntity(this);
    }

    virtual protected void RemovePassThrough(List<GridEntity> winning_objects, List<GridEntity> losing_objects)
    {
        // TODO @matthew/@jay : this does not respect entities with the isAttack flag yet
        GridNode node = winning_objects[0].m_currentNode;

        // if entity is trying to push entity against a wall / edge of grid
        if (winning_objects[0].m_currentNode.Neighbors[winning_objects[0].m_movementDirection].reference == null)
        {
            // if entity is solid, prevent entities from moving (make them stay where they are)
            if (losing_objects[0].m_flags.IsFlagsSet(flags.isSolid))
            {
                losing_objects[0].RemoveFromCurrentNode();
                losing_objects[0].m_currentNode = losing_objects[0].m_previousNode;
                losing_objects[0].m_previousNode = null;
                losing_objects[0].m_targetNode = null;
                losing_objects[0].m_movementDirection = Vector2.zero;
                losing_objects[0].m_speed = 0;

                winning_objects[0].RemoveFromCurrentNode();
                winning_objects[0].m_currentNode = winning_objects[0].m_previousNode;
                winning_objects[0].m_previousNode = null;
                winning_objects[0].m_targetNode = null;
                winning_objects[0].m_movementDirection = Vector2.zero;
                winning_objects[0].m_speed = 0;

                return;
            }

            // losing object gets crushed
            losing_objects[0].m_health = int.MinValue;
            return;
        }

        // move losing object on top of winning object, to be dealt with in resolve move phase
        losing_objects[0].RemoveFromCurrentNode();
        losing_objects[0].m_currentNode = node;
        losing_objects[0].AddToCurrentNode();
        losing_objects[0].m_targetNode = null;
        losing_objects[0].m_speed = 0;
        losing_objects[0].m_movementDirection = Vector2.zero;
    }

    virtual public void RemoveFromCurrentNode()
    {
        if (m_currentNode != null)
            m_currentNode.RemoveEntity(this);
    }

    virtual public void AddToCurrentNode()
    {
        if (m_currentNode != null)
            m_currentNode.AddEntity(this);
    }

    virtual protected void KillSelf()
    {
        RemoveFromCurrentNode();
        m_stepController.RemoveEntity(this);
        GameObject.Destroy(gameObject);
    }

    virtual public void Kill()
    {
        m_flags.SetFlags(flags.isDead, true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, m_movementDirection);
    }
}