using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class CaterpillerEnemy : GridEntity
{
    [SerializeField] private GridEntity m_followEntity; // The next compartment or the player that the enemy is going to be following
    [SerializeField] private bool head; // Variable to tell which compartment is at the front

    private Vector2 m_dir = new Vector2(); // Direction for figuring out movement
    [SerializeField] private int m_moveSpeed = 1; // This enemy will move one square for each step

    private void OnValidate()
    {
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
        m_health = 5; // Set health of compartment to 5
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {

        Vector3[] path; // Declare paht variable

        Vector3 dir = Vector3.zero; // Declare temporary direction variable

        if (!head) // If this compartment is not the front
        {
            // MIGHT WANT TO CHANGE THIS FOR BURROWING MECHANIC
            Vector2 followingNode = new Vector2(Position.world.x, Position.world.y); // Failsafe assigning (line 49). Essentially if the front of the caterpillar is gone the rest of the compartments should just stop moving
            
            if (m_followEntity != null) // If the node exists on the grid
            {
                followingNode = m_followEntity.transform.position; // Find the position of the node it is going to be following
            }
            Vector2 temp = followingNode - new Vector2(Position.world.x, Position.world.y); // Find direction for which way the compartment is meant to be moving
            dir = new Vector3(temp.x, temp.y, 0); // Set direction to value calculated above
            dir = dir.normalized; // Normalize to values between 1 and -1
        }
        else // If it is the front of the head
        {
            path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, m_followEntity.transform.position); // Find path to player
            
            if (path.Length > 1) // If path is greater than 1
            {                
                dir = path[1] - path[0]; // Set direction to be distance vector of the two closest pathfinding nodes
            }
            //else
            //{
            //    Vector2 temp = new Vector2(m_followEntity.transform.position.x, m_followEntity.transform.position.y) - Position.grid;
            //    dir = new Vector3(temp.x, temp.y, 0);
            //    dir = dir.normalized;
            //}


            if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int((int)dir.x, (int)dir.y)), false).Count > 0) // If there are entities infront of the caterpillar
            {
                if (m_currentNode.GetNeighbour(new Vector2Int((int)dir.x, (int)dir.y)).position.world != m_followEntity.transform.position) // If it is not the player
                {
                    dir = CheckDirectionsOfMovement(dir); // Figure out a new direction of movement
                }
            }

        }

        m_dir = new Vector2Int((int)dir.x, (int)dir.y); // Convert direction to integer       
        SetMovementDirection(m_dir, m_moveSpeed); // Set movement
        base.AnalyseStep(); // Run base function
    }

    public override void AttackStep()
    {
        base.AttackStep(); // Run base function
    }

    private Vector2 CheckDirectionsOfMovement(Vector2 direction)
    {

        if(GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count > 0) // If the caterpillar can't move right
        {
            if(GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
            {
                return new Vector2(0, 1); // If it can then return this direction
            }
            else if(GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
            {
                return new Vector2(0, -1); // If it can then return this direction
            }
            else if(GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
            {
                return new Vector2(-1, 0); // If it can then return this direction
            }
        }
        else if(GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count > 0) // If the caterpillar can't move left
        {
            if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
            {
                return new Vector2(0, 1); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
            {
                return new Vector2(0, -1); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
            {
                return new Vector2(1, 0); // If it can then return this direction
            }
        }
        else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count > 0) // If the caterpillar can't move up
        {
            if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
            {
                return new Vector2(1, 0); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
            {
                return new Vector2(-1, 0); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count == 0) // Check if it can move down
            {
                return new Vector2(0, -1); // If it can then return this direction
            }
        }
        else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, -1))).Count > 0) // If the caterpillar can't move down
        {
            if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(1, 0))).Count == 0) // Check if it can move right
            {
                return new Vector2(1, 0); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(-1, 0))).Count == 0) // Check if it can move left
            {
                return new Vector2(-1, 0); // If it can then return this direction
            }
            else if (GetEntitiesOnNode(m_currentNode.GetNeighbour(new Vector2Int(0, 1))).Count == 0) // Check if it can move up
            {
                return new Vector2(0, 1); // If it can then return this direction
            }
        }

        // LIKELY WANT TO CHANGE THIS WHEN IMPLEMENTING BURROW ACTION
        return new Vector2(0, 0); // If there was no solution then just make the caterpillar not move in any direction
    }
}