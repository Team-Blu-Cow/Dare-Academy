using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;
using JUtil;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

[RequireComponent(typeof(GridEntityAnimationController))]
public abstract class GridEntity : MonoBehaviour
{
    // MEMBERS ************************************************************************************
    private Vector2Int m_movementDirection;

    private Vector2Int m_lastMovementDiretion = Vector2Int.zero;

    [SerializeField] protected int m_mass = 2;
    protected int m_baseSpeed = 1;
    private int m_speed = 1;
    [SerializeField] protected int m_health = 1;
    private int m_stepsTaken = 0;
    private bool m_failedAttemptToSwitchRoom = false;

    protected int m_roomIndex = 0;

    [SerializeField] protected GridEntityFlags m_flags = new GridEntityFlags();
    protected GridEntityInternalFlags m_internalFlags = new GridEntityInternalFlags();

    public Vector2Int Direction => m_movementDirection;
    public Vector2Int LastDirection => m_lastMovementDiretion;

    public int Mass { get { return m_mass; } set { m_mass = value; } }
    public int Speed { get { return m_speed; } }
    public int RoomIndex { get { return m_roomIndex; } set { m_roomIndex = value; } }

    // used for controlling the reanalyse process
    public bool doReanalyse
    { get; protected set; }

    private bool forceStepAgain
    { get; set; }

    public GridEntityFlags Flags => m_flags;

    public bool FailedSwitchingRooms => m_failedAttemptToSwitchRoom;

    public int Health
    {
        get { return m_health; }
        set { m_health = value; }
    }

    private int m_startingHealth;

    public bool isDead
    {
        get
        {
            return
                (m_health <= 0 && m_flags.IsFlagsSet(flags.isKillable))
                || m_currentNode == null && !m_flags.IsFlagsSet(flags.allowedOffGrid)
                || m_internalFlags.IsFlagsSet(interalFlags.isDead)
                || (m_currentNode!= null && !m_currentNode.IsTraversable(m_flags.IsFlagsSet(flags.isAirBorn)))
                ; // fuck you Adam, its staying in :] - Love Matthew & Jay
        }
    }

    public bool isPlayer { get { return m_flags.IsFlagsSet(flags.isPlayer); } }

    public bool isFinishedMoving { get { return m_speed <= m_stepsTaken && !forceStepAgain; } }

    protected StepController m_stepController;

    protected GridNode m_currentNode = null;
    protected GridNode m_targetNode = null;
    protected GridNode m_previousNode = null;
    protected GridNode m_startingNode = null;
    protected GridNode m_initialNode = null;

    public bool MovedThisStep
    { get; protected set; }

    public GridNode previousNode
    { get { return m_previousNode; } }

    public GridNode currentNode
    { get { return m_currentNode; } }

    public GridNodePosition Position
    { get { return m_currentNode.position; } }

    protected Queue<GridEnityAction> m_animationQueue;
    protected List<GridEnityAction> m_actionList;
    protected Coroutine m_animationCoroutine;

    [SerializeField, HideInInspector] protected GridEntityAnimationController m_animationController;

    // INITIALISATION METHODS *********************************************************************
    protected virtual void Start()
    {
        doReanalyse = false;

        m_currentNode = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(transform.position);

        m_startingHealth = Health;

        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_currentNode.AddEntity(this);
        m_roomIndex = m_currentNode.roomIndex;

        transform.position = Position.world;

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        m_stepController.RoomChangeEvent += RoomChange;

        if (m_roomIndex == m_stepController.m_currentRoomIndex || Flags.IsFlagsSet(flags.keepAwake))
        {
            m_stepController.AddEntity(this);
        }

        m_startingNode = m_currentNode;

        m_animationQueue = new Queue<GridEnityAction>();
        m_actionList = new List<GridEnityAction>();

        m_speed = m_baseSpeed;

        if (m_animationController == null)
        {
            m_animationController = GetComponent<GridEntityAnimationController>();
            if (m_animationController == null)
                Debug.LogWarning($"[GridEntity] [name = {gameObject.name}] failed to get GridEntityAnimationController");
        }

        AnalyseStep();
    }

    private void OnDestroy()
    {
        m_stepController.RoomChangeEvent -= RoomChange;
        RemoveFromCurrentNode();
    }

    protected virtual void OnValidate()
    {
        m_animationController = GetComponent<GridEntityAnimationController>();
    }

    // STEP FLOW METHODS **************************************************************************

    /*  step flow
     *         [pre-move]
     *         [move]
     * virtual [resolve pass through]
     * virtual [resolve move]
     *         [post-move]
     *         [reanalyse] - exclusivly used by wyrm boss
     *         IF m_stepsTaken != m_speed: GOTO [move]
     * virtual [attack]
     * virtual [damage]
     *         [end]
     * virtual [draw]
     * virtual [analyse]
     *
     */

    public virtual void ResetAnimations()
    {
        if (m_animationCoroutine != null)
            StopCoroutine(m_animationCoroutine);

        m_actionList.Clear();

        m_initialNode = m_currentNode;

        MovedThisStep = false;
    }

    public void PreMoveStep()
    {
        // allows checking if an entity moved this step
        m_previousNode = null;

        // if not on node kill entity, this will prevent next steps from being run
        if (m_currentNode == null && !m_flags.IsFlagsSet(flags.allowedOffGrid))
            Kill();

        if(m_currentNode != null && !m_currentNode.IsTraversable(m_flags.IsFlagsSet(flags.isAirBorn)))
        {
            Kill();
        }
    }

    virtual public void MoveStep()
    {
        if (m_movementDirection != Vector2Int.zero)
        {
            m_lastMovementDiretion = m_movementDirection;
        }

        // set our target node based on our m_moveDirection
        // this should be set within the analysis step
        SetTargetNode(m_movementDirection);

        m_failedAttemptToSwitchRoom = false;
        if (m_currentNode != null && m_targetNode != null)
        {
            // check if entity is allowed to move between rooms and if next tile is in a different room
            if (!m_flags.IsFlagsSet(flags.allowRoomSwitching) && m_currentNode.roomIndex != m_targetNode.roomIndex)
            {
                m_failedAttemptToSwitchRoom = true;
                return;
            }
        }

        // set currentNode to targetNode
        // keep a record of where we came from
        if (m_targetNode != null && m_targetNode.IsTraversable(m_flags.IsFlagsSet(flags.isAirBorn)))
        {
            // remove ourself from currentNode
            // previous and target node is overkill to account for sloppy programing
            if (m_previousNode != null)
                m_previousNode.RemoveEntity(this);
            m_currentNode.RemoveEntity(this);
            m_targetNode.RemoveEntity(this);

            // update our position
            m_previousNode = m_currentNode;
            m_currentNode = m_targetNode;
            m_targetNode = null;

            AddAnimationAction(m_currentNode.position, ActionTypes.MOVE, "WalkBlend");
            m_animationController.animator.SetFloat("WalkDirX", m_movementDirection.x);
            m_animationController.animator.SetFloat("WalkDirY", m_movementDirection.y);

            // add ourself to the list of entities currently on the node
            m_currentNode.AddEntity(this);
        }
    }

    virtual public void ResolvePassThroughStep()
    {
        if (!CheckForPassThrough())
            return;

        List<GridEntity> winningEntities = GetEntitiesOnNode(m_previousNode);
        List<GridEntity> losingEntities = new List<GridEntity>();
        winningEntities.Add(this);

        ResolveFlagsConflict(ref winningEntities, ref losingEntities);
        if (winningEntities.Count == 1)
        {
            RemovePassThrough(winningEntities, losingEntities);
        }

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
                highestSpeed = (winningEntities[i].Speed > winningEntities[i + 1].Speed) ? winningEntities[i].Speed : winningEntities[i + 1].Speed;
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

            entity.ModifyPreviousAction(ActionTypes.PASSTHROUGH, true);

            entity.AddAnimationAction(ActionTypes.MOVE, "WalkBlend");

            entity.m_speed = 0;
            entity.m_movementDirection = Vector2Int.zero;
        }
    }

    virtual public void ResolveMoveStep()
    {
        if (m_currentNode == null)
            return;

        // when multiple entities are on the same tile:
        // all objects will be placed in winning_objects
        // each stage of the collision resolution process will move objects to losing_objects until only 1 remains
        // every object that is contained within losing_objects either returns to its starting point or is pushed by the sole remaining object in winning_objects

        List<GridEntity> winning_objects; // everything
        List<GridEntity> losing_objects = new List<GridEntity>();

        winning_objects = GetEntitiesOnNode(m_currentNode);

        // check for conflict on current node
        if (!CheckForConflict())
            return;

        if (winning_objects.Count > 1)
        {
            // check for any flags that force entities to always win
            ResolveFlagsConflict(ref winning_objects, ref losing_objects);
        }

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
            // if a solution cant be found pick a random object
            // in reality, the object picked is always the first one to be initialised by the engine
            ResolveRandomConflict(ref winning_objects, ref losing_objects);
        }

        if (winning_objects.Count == 1)
        {
            bool success = true;
            for (int i = losing_objects.Count - 1; i >= 0; i--)
            {
                if (losing_objects[i].m_previousNode != null)
                {
                    losing_objects[i].MoveBack();
                }
                else
                {
                    if (!losing_objects[i].PushBack(winning_objects[0].Direction, winning_objects[0].Mass))
                    {
                        success = false;
                    }
                }
            }
            if (!success)
            {
                winning_objects[0].MoveBack();
            }
        }

        // check for any new conflicts
    }

    public void PostMoveStep()
    {
        if (currentNode != previousNode)
            MovedThisStep = true;

        forceStepAgain = false;

        if (m_currentNode != null && !m_flags.IsFlagsSet(flags.allowedOffGrid))
        {
            List<GridEntity> entities = m_currentNode.GetGridEntities();
            if (!entities.Contains(this))
            {
                Debug.LogWarning($"{gameObject.name} : entity was not on list, adding to node list");
                AddToCurrentNode();
            }
        }

        if (m_flags.IsFlagsSet(flags.killOnRoomSwitch) && FailedSwitchingRooms)
        {
            Kill();
        }

        if(m_currentNode != null && !m_currentNode.IsTraversable(m_flags.IsFlagsSet(flags.isAirBorn)))
        {
            m_stepsTaken = int.MaxValue;
            m_movementDirection = Vector2Int.zero;
            Kill();
        }

        // iterate counter of steps taken this turn, this is reset in End()
        m_stepsTaken++;

        if (m_stepsTaken >= m_speed)
        {
            m_movementDirection = Vector2Int.zero;
        }
    }

    public virtual void ReAnalyseStep()
    {
        doReanalyse = false;
        forceStepAgain = true;
    }

    virtual public void AttackStep()
    {
    }

    virtual public void DamageStep()
    {
    }

    virtual public void EndStep()
    {
        doReanalyse = false;

        m_stepsTaken = 0;
        m_speed = 0;

        // someone messed up bad, people are inside each other, lets clean them up
        if (m_currentNode != null && m_currentNode.GetGridEntities().Count > 1)
        {
            List<GridEntity> entities = GetEntitiesOnNode(m_currentNode);

            // remove dead entities from tile, we don't care about them
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].isDead)
                {
                    entities[i].Kill();
                    entities.RemoveAt(i);
                }
            }

            if (entities.Count > 1)
            {
                // only log after removing the dead
                Debug.LogWarning("[GridEntity] - EndStep(): multiple entities on the same tile at end of turn");

                bool playerPresent = false;
                int highestMass = int.MinValue;

                // find what the highest mass of an entity is
                foreach (var entity in entities)
                {
                    if (entity.Mass > highestMass)
                    {
                        highestMass = entity.Mass;
                        if (entity.isPlayer)
                        {
                            playerPresent = true;
                            break; // we don't care about the highest mass anymore
                        }
                    }
                }

                // if player is on tile, they take absolute priority
                if (playerPresent)
                {
                    for (int i = entities.Count - 1; i >= 0; i--)
                    {
                        if (!entities[i].isPlayer)
                        {
                            entities[i].Kill();
                            entities.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    // remove all entities that are not of highest mass
                    for (int i = entities.Count - 1; i >= 0; i--)
                    {
                        if (entities[i].Mass < highestMass)
                        {
                            entities[i].Kill();
                            entities.RemoveAt(i);
                        }
                    }

                    // all other options have failed to solve issue - only leave the entity at entities[0] alive
                    for (int i = entities.Count - 1; i > 0; i--)
                    {
                        entities[i].Kill();
                        entities.RemoveAt(i);
                    }
                }
            }
        }

        const interalFlags fl =
            interalFlags.refectBullets_N |
            interalFlags.refectBullets_E |
            interalFlags.refectBullets_S |
            interalFlags.refectBullets_W |
            interalFlags.refectBullets_NE |
            interalFlags.refectBullets_NW |
            interalFlags.refectBullets_SE |
            interalFlags.refectBullets_SW;

        m_internalFlags.SetFlags(fl, false);

        if (isDead)
        {
            // kill entity
            OnDeath();
        }
    }

    virtual public void AnalyseStep()
    {
    }

    virtual public void DrawStep()
    {
        if (m_currentNode == null)
            return;

        //StartCoroutine(AnimateMove(m_stepController.m_stepTime));

        //return;

        if (m_actionList.Count < 1)
            return;

        if (m_baseSpeed > 1)
        {
            int passthroughCount = 0;
            int pushbackCount = 0;

            for (int i = m_actionList.Count - 1; i >= 1; i--)
            {
                if (m_actionList[i - 1].type == ActionTypes.PASSTHROUGH)
                    passthroughCount++;

                if (m_actionList[i - 1].type == ActionTypes.PUSHBACK)
                    pushbackCount++;

                if (passthroughCount > 1 && m_actionList[i].type == ActionTypes.MOVE)
                    m_actionList.RemoveAt(i);

                if (pushbackCount > 1 && m_actionList[i].type == ActionTypes.MOVE)
                    m_actionList.RemoveAt(i);
            }
        }

        m_animationCoroutine = StartCoroutine(AnimateActions());

        m_baseSpeed = 0;
    }

    virtual public void OnDeath()
    {
        CleanUp();
    }

    // RESOLVE MOVE CONFLICT METHODS **************************************************************
    virtual public bool CheckForConflict()
    {
        // check for conflict on current node
        if (m_currentNode != null)
        {
            List<GridEntity> entities = GetEntitiesOnNode(m_currentNode);
            if (entities.Count > 1)
                return true;
        }
        return false;
    }

    virtual public bool CheckForPassThrough()
    {
        // get node behind entity
        // GridNode behindNode = m_currentNode.Neighbors[(-m_movementDirection).RotationToIndex(45)].reference;
        GridNode behindNode = m_previousNode;

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

    virtual protected void ResolveFlagsConflict(ref List<GridEntity> winning_objects, ref List<GridEntity> losing_objects)
    {
        bool alwaysWin = false;
        foreach (GridEntity entity in winning_objects)
        {
            if (entity.Flags.IsFlagsSet(flags.alwaysWinConflict))
            {
                alwaysWin = true;
                break;
            }
        }

        if (alwaysWin)
        {
            for (int i = winning_objects.Count - 1; i >= 0; i--)
            {
                if (!winning_objects[i].Flags.IsFlagsSet(flags.alwaysWinConflict))
                {
                    losing_objects.Add(winning_objects[i]);
                    winning_objects.RemoveAt(i);
                }
            }
        }
    }

    virtual public bool MoveBack()
    {
        RemoveFromCurrentNode();
        m_currentNode = m_previousNode;
        AddToCurrentNode();
        ModifyPreviousAction(ActionTypes.PUSHBACK, true);
        AddAnimationAction(ActionTypes.MOVE, "WalkBlend");
        m_previousNode = null;
        return true;
    }

    virtual public bool PushBack(Vector2Int direction, int force)
    {
        if (force < Mass)
            return false;

        if (m_currentNode == null)
            return true;

        if (!m_flags.IsFlagsSet(flags.isPushable))
        {
            return false;
        }

        GridNode node = m_currentNode.GetNeighbour(direction);
        if (node == null)
        {
            if (!m_flags.IsFlagsSet(flags.isSolid))
            {
                RemoveFromCurrentNode();
                m_currentNode = null;
                return true;
            }
            return false;
        }

        if(!node.IsTraversable(m_flags.IsFlagsSet(flags.isAirBorn)))
        {
            RemoveFromCurrentNode();
            m_currentNode = null;
            return true;
        }

        bool return_value = true;

        RemoveFromCurrentNode();

        GridNode lastNode = m_currentNode;
        m_currentNode = node;

        List<GridEntity> entities = m_currentNode.GetGridEntities();

        for (int i = entities.Count - 1; i >= 0; i--)
        {
            bool success = entities[i].PushBack(direction, force - entities[i].Mass);

            if (!success)
            {
                m_currentNode = lastNode;
                return_value = false;
            }
        }

        if (return_value)
        {
            ModifyPreviousAction(ActionTypes.PUSHBACK, true);
            AddAnimationAction(m_currentNode, ActionTypes.MOVE, "WalkBlend");
        }

        AddToCurrentNode();
        return return_value;
    }

    virtual protected void RemovePassThrough(List<GridEntity> winning_objects, List<GridEntity> losing_objects)
    {
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
                losing_objects[0].m_movementDirection = Vector2Int.zero;
                losing_objects[0].m_speed = 0;
                losing_objects[0].AddToCurrentNode();
                losing_objects[0].ModifyPreviousAction(ActionTypes.PASSTHROUGH, true);
                losing_objects[0].AddAnimationAction(ActionTypes.MOVE, "WalkBlend");

                winning_objects[0].RemoveFromCurrentNode();
                winning_objects[0].m_currentNode = winning_objects[0].m_previousNode;
                winning_objects[0].m_previousNode = null;
                winning_objects[0].m_targetNode = null;
                winning_objects[0].m_movementDirection = Vector2Int.zero;
                winning_objects[0].m_speed = 0;
                winning_objects[0].AddToCurrentNode();
                winning_objects[0].ModifyPreviousAction(ActionTypes.PASSTHROUGH, true);
                winning_objects[0].AddAnimationAction(ActionTypes.MOVE, "WalkBlend");

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
        losing_objects[0].m_previousNode = null;
        losing_objects[0].m_targetNode = null;
        losing_objects[0].m_speed = 0;
        losing_objects[0].m_movementDirection = Vector2Int.zero;
        losing_objects[0].ModifyPreviousAction(ActionTypes.PASSTHROUGH, true);
        losing_objects[0].AddAnimationAction(ActionTypes.MOVE, "WalkBlend");
    }

    // DRAW STEP METHODS **************************************************************************

    public IEnumerator AnimateActions()
    {
        float animTime = m_stepController.stepTime / m_actionList.Count;

        int count = m_actionList.Count;

        //m_actionList.Reverse();

        for (int i = 0; i < count; i++)
        {
            GridEnityAction action = m_actionList[0];
            m_actionList.RemoveAt(0);

            yield return StartCoroutine(AnimateAction(animTime, action));

            yield return null;
        }
    }

    public IEnumerator AnimateAction(float animTime, GridEnityAction action)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = action.targetPosition;

        switch (action.type)
        {
            case ActionTypes.PASSTHROUGH:
                endPos = Vector3.Lerp(startPos, endPos, 0.15f);
                animTime *= 0.33f;
                break;

            case ActionTypes.PUSHBACK:
                endPos = Vector3.Lerp(startPos, endPos, 0.35f);
                animTime *= 0.55f;
                break;
        }

        float currentTime = 0;

        if (m_animationController != null)
        {
            m_animationController.PlayAnimation(action.animationName, animTime, action.animationLayer);
        }

        while (currentTime < animTime)
        {
            currentTime += Time.deltaTime;

            float xx = Mathf.Lerp(startPos.x, endPos.x, currentTime / animTime);
            float yy = Mathf.Lerp(startPos.y, endPos.y, currentTime / animTime);
            float zz = Mathf.Lerp(startPos.z, endPos.z, currentTime / animTime);
            transform.position = new Vector3(xx, yy, zz);

            yield return null;
        }
    }

    public IEnumerator AnimateMove(float stepTime)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = m_currentNode.position.world;

        float currentTime = 0;

        while (currentTime < stepTime)
        {
            currentTime += Time.deltaTime;

            float xx = Mathf.Lerp(startPos.x, endPos.x, currentTime / stepTime);
            float yy = Mathf.Lerp(startPos.y, endPos.y, currentTime / stepTime);
            float zz = Mathf.Lerp(startPos.z, endPos.z, currentTime / stepTime);
            transform.position = new Vector3(xx, yy, zz);

            yield return null;
        }
    }

    public void AddAnimationAction(GridNode node, ActionTypes type, string animationName, int layer = -1) => AddAnimationAction(node.position, type, animationName, layer);

    public void AddAnimationAction(ActionTypes type, string animationName, int layer = -1)
    {
        if (m_currentNode != null)
            AddAnimationAction(m_currentNode.position, type, animationName, layer);
    }

    public void AddAnimationAction(GridNodePosition position, ActionTypes type, string animationName, int layer = -1)
    {
        GridEnityAction action = new GridEnityAction();
        action.position = position;
        action.type = type;
        action.animationName = animationName;
        action.animationLayer = layer;
        m_actionList.Add(action);
    }

    public void ModifyPreviousAction(ActionTypes type, bool conditional = false, ActionTypes condition = ActionTypes.MOVE)
    {
        if (m_actionList.Count <= 0)
            return;

        if (!conditional || m_actionList[m_actionList.Count - 1].type == condition)
            m_actionList[m_actionList.Count - 1].type = type;

        //m_animationQueue.Enqueue(action);
    }

    public void ModifyPreviousAction(GridEnityAction action, bool conditional = false, ActionTypes condition = ActionTypes.MOVE)
    {
        if (m_actionList.Count <= 0)
            return;

        if (!conditional || m_actionList[m_actionList.Count - 1].type == condition)
            m_actionList[m_actionList.Count - 1] = new GridEnityAction(action);

        //m_animationQueue.Enqueue(action);
    }

    // HELPER METHODS *****************************************************************************

    virtual public void OnHit(int damage, float offsetTime = 0f)
    {
        m_health -= damage;
    }

    protected void UpdateTransform()
    {
        if (m_currentNode != null)
            transform.position = m_currentNode.position.world;
    }

    virtual public void RoomChange()
    {
        if (m_roomIndex == m_stepController.m_currentRoomIndex || Flags.IsFlagsSet(flags.keepAwake))
        {
            m_stepController.AddEntity(this);
        }
        else
        {
            ResetPosition();
            m_stepController.RemoveEntity(this);
        }
    }

    protected void ResetPosition()
    {
        if (m_flags.IsFlagsSet(flags.destroyOnReset))
        {
            Kill();
        }
        else if (m_flags.IsFlagsSet(flags.dontMoveOnReset))
        {
        }
        else
        {
            RemoveFromCurrentNode();
            m_currentNode = m_startingNode;
            m_health = m_startingHealth;
            AddToCurrentNode();
            UpdateTransform();
        }
    }

    public void SetMovementDirection(Vector2 direction, int speed = 1)
    {
        Vector2Int dirInt = new Vector2Int((int)direction.x, (int)direction.y);

        SetMovementDirection(dirInt, speed);
    }

    public void SetMovementDirection(Vector2Int direction, int speed = 1)
    {
        if (direction == null)
        {
            Debug.LogWarning($"[GridEnitity.SetMovementDirection] [Name = {gameObject.name}], direction was null");
            return;
        }

        if (speed < 0)
        {
            Debug.LogWarning($"[GridEnitity.SetMovementDirection] [Name = {gameObject.name}], speed was less than 0");
            speed = 0;
        }

        if (direction.x > 0)
            direction.x = 1;
        else if (direction.x < 0)
            direction.x = -1;

        if (direction.y > 0)
            direction.y = 1;
        else if (direction.y < 0)
            direction.y = -1;

        m_movementDirection = direction;
        m_speed = speed;
        m_baseSpeed = speed;
    }

    protected void SetTargetNode(Vector2Int direction, int distance = 1)
    {
        if (direction == Vector2Int.zero)
            return;

        m_movementDirection = direction;

        int dir = direction.RotationToIndex(45);
        Mathf.Clamp(dir, 0, 7);

        m_previousNode = null;

        m_targetNode = m_currentNode;

        for (int i = 0; i < distance; i++)
        {
            GridNode node = m_targetNode.GetNeighbour(direction);
            if (node != null)
                m_targetNode = node;
        }

        if (m_targetNode == m_currentNode) // if entity is not moving
        {
            m_targetNode = null;
            m_movementDirection = Vector2Int.zero;
            m_speed = 0;
        }
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

    virtual public void CleanUp()
    {
        RemoveFromCurrentNode();
        m_stepController.RemoveEntity(this);
        GameObject.Destroy(gameObject);
    }

    virtual public void Kill()
    {
        m_internalFlags.SetFlags(interalFlags.isDead, true);
    }

    virtual public void KillImmediate()
    {
        m_internalFlags.SetFlags(interalFlags.isDead, true);
        RemoveFromCurrentNode();

        SpriteRenderer spriteRenderer;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.enabled = false;
            spriteRenderer = null;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.enabled = false;
            spriteRenderer = null;
        }
    }

    protected List<GridEntity> GetEntitiesOnNode(GridNode node, bool discardAttacks = true)
    {
        if (node == null)
            return new List<GridEntity>();

        List<GridEntity> entities = new List<GridEntity>(node.GetGridEntities());
        if (discardAttacks)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].m_flags.IsFlagsSet(flags.isAttack))
                {
                    entities.RemoveAt(i);
                }
            }
        }

        return entities;
    }

    protected bool SpawnBullet(GameObject prefab, GridNode sourceNode, Vector2 direction, int damage = 1)
    {
        if (direction == null)
            return false;

        Vector2Int dir = new Vector2Int((int)direction.x, (int)direction.y);
        return SpawnBullet(prefab, sourceNode, dir, damage);
    }

    protected bool SpawnBullet(GameObject prefab, GridNode sourceNode, Vector2Int direction, int damage = 1)
    {
        return SpawnBullet(out GameObject bullet, prefab, sourceNode, direction, damage);
    }

    protected bool SpawnBullet(out GameObject bullet, GameObject prefab, GridNode sourceNode, Vector2 direction, int damage = 1)
    {
        if (direction == null)
        {
            bullet = null;
            return false;
        }

        Vector2Int dir = new Vector2Int((int)direction.x, (int)direction.y);
        return SpawnBullet(out bullet, prefab, sourceNode, dir, damage);
    }

    protected bool SpawnBullet(out GameObject bullet, GameObject prefab, GridNode sourceNode, Vector2Int direction, int damage = 1)
    {
        if (sourceNode == null)
        {
            bullet = null;
            return false;
        }

        if (direction == null)
        {
            bullet = null;
            return false;
        }

        if (prefab)
        {
            GridNode spawnNode = sourceNode.GetNeighbour(direction); ;

            if (spawnNode == null)
            {
                bullet = null;
                return false;
            }

            if (sourceNode.roomIndex != spawnNode.roomIndex)
            {
                bullet = null;
                return false;
            }

            List<GridEntity> entities = spawnNode.GetGridEntities();

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].TryGetComponent<GridEntity>(out GridEntity other))
                {
                    if (other is BulletEntity)
                    {
                        bullet = null;
                        return false;
                    }

                    if (other.Flags.IsFlagsSet(flags.isAttack))
                    {
                        entities.RemoveAt(i);
                    }
                }
            }
            Vector3 spawnPosition = spawnNode.position.world;

            if (entities.Count > 0)
            {
                foreach (GridEntity entity in entities)
                {
                    //entity.m_health -= damage;
                    entity.OnHit(damage);
                }

                if (prefab.TryGetComponent(out BulletEntity bulletEntity))
                {
                    if (bulletEntity.ExplosionPrefab != null)
                    {
                        BulletEntity.DeathExplosion(bulletEntity.ExplosionPrefab, spawnPosition);
                    }
                }

                bullet = null;
                return true;
            }

            bullet = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
            if (bullet)
            {
                BulletEntity bulletEntity = bullet.GetComponent<BulletEntity>();

                if (bulletEntity)
                {
                    bulletEntity.m_damage = damage;
                    bulletEntity.m_bulletDirection = direction;
                    return true;
                }
            }
        }

        bullet = null;
        return false;
    }

    public bool ShouldReflectBullet(Vector2Int incomingDirection)
    {
        if (m_flags.IsFlagsSet(flags.refectAllBullets))
        { return true; }

        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_S) && incomingDirection == Vector2Int.up)
        { return true; }

        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_W) && incomingDirection == Vector2Int.right)
        { return true; }

        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_E) && incomingDirection == Vector2Int.left)
        { return true; }

        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_N) && incomingDirection == Vector2Int.down)
        { return true; }

        Vector2Int vec = new Vector2Int();

        vec.x = 1; vec.y = 1;
        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_SW) && incomingDirection == vec)
        { return true; }

        vec.x = -1; vec.y = 1;
        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_SE) && incomingDirection == vec)
        { return true; }

        vec.x = 1; vec.y = -1;
        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_NW) && incomingDirection == vec)
        { return true; }

        vec.x = -1; vec.y = -1;
        if (m_internalFlags.IsFlagsSet(interalFlags.refectBullets_NE) && incomingDirection == vec)
        { return true; }

        return false;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector2 rayVec = new Vector2(m_movementDirection.x, m_movementDirection.y);
        Gizmos.DrawRay(transform.position, rayVec);
    }
}