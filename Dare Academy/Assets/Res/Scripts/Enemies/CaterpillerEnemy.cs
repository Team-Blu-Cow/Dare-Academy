using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class CaterpillerEnemy : GridEntity
{
    [SerializeField] private GridEntity m_followEntity; // The next compartment or the player that the enemy is going to be following
    [SerializeField] private bool head; // Variable to tell which compartment is at the front

    [SerializeField] private Vector2 m_dir = new Vector2(); // Direction for figuring out movement
    [SerializeField] private int m_moveSpeed = 1; // This enemy will move one square for each step

    public bool isBurrowing = false;
    public bool isTelegraphBurrow = false;
    public Vector3 newPos = Vector3.zero;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (m_followEntity && m_followEntity.TryGetComponent(out CaterpillerEnemy caterpiller)) // If the following entity has the caterpillar script
        {
            head = false; // Then it is not at the front
        }
        else // If it doesn't. aka its following the player
        {
            head = true; // Then this compartment is the front of the caterpillar
        }
    }

    protected override void Start()
    {
        base.Start(); // Run base application
        m_health = 1; // Set health of compartment to 1
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep(); // Run base function
        Vector3[] path; // Declare paht variable

        Vector3 dir = Vector3.zero; // Declare temporary direction variable

        if (!head) // If this compartment is not the front
        {
            // MIGHT WANT TO CHANGE THIS FOR BURROWING MECHANIC
            Vector2 followingNode = new Vector2(Position.world.x, Position.world.y);

            if (m_followEntity != null) // If the entity exists on the grid
            {
                followingNode = m_followEntity.transform.position; // Find the position of the node it is going to be following
            }
            Vector2 temp = followingNode - new Vector2(Position.world.x, Position.world.y); // Find direction for which way the compartment is meant to be moving
            dir = new Vector3(temp.x, temp.y, 0); // Set direction to value calculated above
            dir = dir.normalized; // Normalize the values between 1 and -1
        }
        else // If it is the front of the head
        {
            if (!isBurrowing)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(false);

                path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, m_followEntity.transform.position); // Find path to player

                if (path.Length > 1) // If path is greater than 1
                {
                    dir = path[1] - path[0]; // Set direction to be distance vector of the two closest pathfinding nodes
                }

                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int((int)dir.x, (int)dir.y)), false).Count > 0) // If there are entities infront of the caterpillar
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int((int)dir.x, (int)dir.y)).position.world != m_followEntity.transform.position) // If it is not the player
                    {
                        dir = CheckDirectionsOfMovement(dir); // Figure out a new direction of movement
                    }
                }

                if (dir == Vector3.zero)
                {
                    isBurrowing = true;
                }
            }
            else
            {
                if (isBurrowing && isTelegraphBurrow)
                {
                    BurrowMove();
                }

                if (!isTelegraphBurrow)
                {
                    TelegraphBurrow();
                }
            }            
        }

        m_dir = new Vector2Int((int)dir.x, (int)dir.y); // Convert direction to integer
        SetMovementDirection(m_dir, m_moveSpeed); // Set movement
    }

    public override void MoveStep()
    {
        if (!isBurrowing && !transform.parent.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing)
            base.MoveStep();
    }

    public override void ResolveMoveStep()
    {
        if (!isBurrowing && !transform.parent.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing)
            base.ResolveMoveStep();
    }

    public override void ResolvePassThroughStep()
    {
        if (!isBurrowing && !transform.parent.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing)
            base.ResolvePassThroughStep();
    }

    public override void AddToCurrentNode()
    {
        if (!isBurrowing && !transform.parent.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing)
            base.AddToCurrentNode();
    }

    public override void AttackStep()
    {
        base.AttackStep(); // Run base function
    }

    private Vector2 CheckDirectionsOfMovement(Vector2 direction)
    {
        if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count > 0) // If the caterpillar can't move right
        {
            if (m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
                {
                    return new Vector2(0, 1); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
                {
                    return new Vector2(0, -1); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
                {
                    return new Vector2(-1, 0); // If it can then return this direction
                }
            }
        }
        else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count > 0) // If the caterpillar can't move left
        {
            if (m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
                {
                    return new Vector2(0, 1); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
                {
                    return new Vector2(0, -1); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
                {
                    return new Vector2(1, 0); // If it can then return this direction
                }
            }
        }
        else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count > 0) // If the caterpillar can't move up
        {
            if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
                {
                    return new Vector2(1, 0); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
                {
                    return new Vector2(-1, 0); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
                {
                    return new Vector2(0, -1); // If it can then return this direction
                }
            }
        }
        else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count > 0) // If the caterpillar can't move down
        {
            if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
                {
                    return new Vector2(1, 0); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
                {
                    return new Vector2(-1, 0); // If it can then return this direction
                }
            }
            else if (m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null)
            {
                if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
                {
                    return new Vector2(0, 1); // If it can then return this direction
                }
            }
        }

        // LIKELY WANT TO CHANGE THIS WHEN IMPLEMENTING BURROW ACTION
        return new Vector2(0, 0); // If there was no solution then just make the caterpillar not move in any direction
    }

    private void TelegraphBurrow()
    {
        transform.GetChild(0).gameObject.transform.localPosition = Vector3.zero;
        bool foundPos = false;
        int burrowRange = 7;

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);

        while (foundPos == false)
        {
            newPos = new Vector3(Random.Range(transform.position.x - burrowRange, transform.position.x + burrowRange), Random.Range(transform.position.y - burrowRange, transform.position.y + burrowRange), 0);
            newPos = new Vector3Int((int)newPos.x, (int)newPos.y, (int)newPos.z);
            newPos = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f, 0);

            if (blu.App.GetModule<blu.LevelModule>().Grid(RoomIndex).NodeExistsAt(newPos))
            {
                Debug.Log("Node exists");
                foundPos = true;
            }
        }

        isTelegraphBurrow = true;
    }

    private void BurrowMove()
    {
        Vector3 distanceVector = transform.GetChild(0).gameObject.transform.position - newPos;

        if (distanceVector.x > 0)
        {
            m_currentNode = m_currentNode.GetNeighbour(new Vector2Int(-1, 0));
            //transform.GetChild(0).gameObject.transform.position += new Vector3(-1, 0, 0);
            return;
        }
        else if (distanceVector.x < 0)
        {
            m_currentNode = m_currentNode.GetNeighbour(new Vector2Int(1, 0));
            //transform.GetChild(0).gameObject.transform.position += new Vector3(1, 0, 0);
            return;
        }
        else if (distanceVector.y > 0)
        {
            m_currentNode = m_currentNode.GetNeighbour(new Vector2Int(0, -1));
            //transform.GetChild(0).gameObject.transform.position += new Vector3(0, -1, 0);
            return;
        }
        else if (distanceVector.y < 0)
        {
            m_currentNode = m_currentNode.GetNeighbour(new Vector2Int(0, 1));
            //transform.GetChild(0).gameObject.transform.position += new Vector3(0, 1, 0);
            return;
        }

        if (m_currentNode.position.world == newPos)
        {           
            isTelegraphBurrow = false;
            isBurrowing = false;
        }
    }

    protected override void OnDrawGizmos()
    {
        if (!isTelegraphBurrow || !isBurrowing) // If the entity is not burrowing
        {
            Gizmos.color = new Color(1, 1, 1, 1);
        }
        else // If the entity is burrowing
        {
            if (isBurrowing)
            {
                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1f); // Set color of boxes to be a transparent yellow
                Gizmos.DrawLine(transform.position, newPos);
            }
        }
    }
}