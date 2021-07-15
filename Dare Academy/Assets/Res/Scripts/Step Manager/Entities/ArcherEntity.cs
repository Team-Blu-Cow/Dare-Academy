using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;
using JUtil.Grids;

public class ArcherEntity : GridEntity
{
    [Header("Attack attributes")]
    [SerializeField] private int m_attackCooldown = 5; // Cooldown after firing a bullet

    [SerializeField] private int agroRange = 5; // Range for how far away the entity is allowed to start attacking the player
    [SerializeField] private int m_cooldownCounter = 0; // Cooldown counter (monitors how long it has been since the last buller has been fired)
    private bool isAttacking = false; // Boolean for if the entity is attacking or not
    private bool isWaiting = false; // Boolean for if the entity is waiting after firing a bullet

    [Header("Move attributes")]
    [SerializeField] private int moveSpeed = 1; // Set move speed to 1. Travels 1 square each step

    private Vector2 m_dir = new Vector2(); // Set direction of movement

    [Header("Resources needed")]
    [SerializeField] private GameObject m_bulletPrefab = null; // Bullet prefab for spawning bullets

    [SerializeField] private GridEntity player; // Player for finding path // WILl NEED TO BE CHANGED TO NAME OF PLAYER

    private Vector3[] path;

    [Header("Jays rework stuff")]
    [SerializeField] Vector2Int positionDiff;
    [SerializeField] Vector2 offsetVector;
    [SerializeField] GridNodePosition targetPosition;
    [SerializeField] int distance;
    [SerializeField] int dist;
    [SerializeField] int fearRange = 3;
    bool pathBlocked = false;

    public enum State
    {
        MOVING,
        SHOOTING,
        FLEEING
    }


    protected override void Start()
    {
        base.Start(); // Run base start function
        m_health = 5; // Set health
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true); // Set flag for killable to true
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true); // Set flag for if solid to true
        player = FindObjectOfType<PlayerEntity>();
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet"); // Find bullet prefab
    }

    public override void AnalyseStep()
    {
        if (player == null)
            return;

        base.AnalyseStep(); // Run base function

        MoveState();
    }

    public void MoveState()
    {
        positionDiff = player.Position.grid - this.Position.grid;

        if (positionDiff.x == 0 || positionDiff.y == 0) // If player and entity are aligned on grid
        {
            //if ((distanceVector.x <= 4.0f && distanceVector.x >= -4.0f) && (distanceVector.y <= 4.0f && distanceVector.y >= -4.0f)) // If entity is within range of player
            //{
            if (m_currentNode.IsPathClear(player.currentNode)) // If player is not behind a wall
            {
                if (m_cooldownCounter <= 0) // If the cooldown has expired
                {
                    m_cooldownCounter = m_attackCooldown; // Reset cooldown
                    isAttacking = true; // Set is attacking to true
                }
            }
            //}
        }

        offsetVector = new Vector2(
            (Mathf.Abs(positionDiff.x) > Mathf.Abs(positionDiff.y)) ? positionDiff.x : 0,
            (Mathf.Abs(positionDiff.y) >= Mathf.Abs(positionDiff.x)) ? positionDiff.y : 0
            );

        GridNode targetNode;

        targetNode = player.currentNode.GetNeighbourDist(Vector2Int.RoundToInt(-offsetVector), Mathf.RoundToInt(offsetVector.magnitude));

        if (targetNode != null)
            targetPosition = targetNode.position;

        dist = player.currentNode.GetDistanceInDirection(Vector2Int.RoundToInt(-offsetVector));

        distance = Mathf.RoundToInt(offsetVector.magnitude);

        pathBlocked = false;

        if (dist < distance)
            if (dist <= fearRange)
                pathBlocked = true;

        if (pathBlocked)
        {
            if (Mathf.Abs(offsetVector.x) > Mathf.Abs(offsetVector.y))
                offsetVector = new Vector2(0, (positionDiff.y == 0) ? 3 : positionDiff.y);
            else
                offsetVector = new Vector2((positionDiff.x == 0) ? 3 : positionDiff.x, 0);

            dist = player.currentNode.GetDistanceInDirection(Vector2Int.RoundToInt(-offsetVector));

            if (dist < fearRange)
            {
                offsetVector = -offsetVector;

                // do checks here
            }

            targetNode = player.currentNode.GetNeighbourDist(Vector2Int.RoundToInt(-offsetVector), fearRange);

            if (targetNode != null)
                targetPosition = targetNode.position;

        }

        path = new Vector3[0];
        path = App.GetModule<LevelModule>().MetaGrid.GetPathWithFear(Position.world, targetPosition.world, player.Position.world, fearRange);

        if (!isAttacking && !isWaiting) // If the entity is not attacking and not waiting
        {
            if (path.Length <= agroRange) // If the player is within agro range of entity
            {
                if (path.Length > 1) // If the path array has more than 1 value stored
                {
                    m_dir = path[1] - path[0]; // Find the direction the entity is meant to move in
                }

                m_dir = new Vector2Int((int)m_dir.x, (int)m_dir.y); // Set direction to int values
                SetMovementDirection(m_dir, moveSpeed); // Set movement speed and direction
            }
        }
        else // If the player is attacking
        {
            isWaiting = false; // Set waiting to be false
            SetMovementDirection(Vector2.zero, moveSpeed); // Don't make the entity move this step
        }

        if (m_cooldownCounter > 0) // If the cooldown hasn't expired yet
        {
            m_cooldownCounter--; // Decrement cooldown counter
        }//*/
    }

    public override void AttackStep()
    {
        if (isAttacking == true) // If the entity is attacking
        {
            SpawnBullet(m_bulletPrefab, m_currentNode, m_dir); // Spawn bullet in the direction the entity was moving
            isAttacking = false; // Set attacking to false
            isWaiting = true; // Make the entity not move after this by setting is waiting to true
        }
    }

    private bool CheckPlayerIsBehindWall(Vector2 direction) // Function check for if the player is behind a wall, important for checking whether the entity can fire at them or not
    {
        Vector2Int m_direction = new Vector2Int((int)direction.x, (int)direction.y); // Create vector int variable for the direction the entity is facing

        // Hopefully this makes sense, essentially just going in an imaginary line in the direction of where the entity is facing and checking if there is a wall or not.
        if (m_currentNode.GetNeighbour(m_direction) == null) // If the node infront of the entity is null
        {
            return true; // Then it is facing a wall
        }
        else if (m_currentNode.GetNeighbour(m_direction).GetNeighbour(m_direction) == null) // Check the node in the direction of the entity of that neighbour for whether it is null
        {
            return true; // Then the player is behind a wall
        }
        else if (m_currentNode.GetNeighbour(m_direction).GetNeighbour(m_direction).GetNeighbour(m_direction) == null) // Check the node in the direction of the entity of that neighbour for whether it is null
        {
            return true; // Then the player is behind a wall
        }
        else
        {
            return false; // Then the player isn't behind a wall
        }
    }

    protected override void OnDrawGizmos()
    {
        if (!isAttacking) // If the entity is not attacking
        {
            Gizmos.color = new Color(0, 0, 0, 0); // Set boxes to be transparent color
        }
        else // If the entity is attacking
        {
            Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f); // Set color of boxes to be a transparent yellow
        }

        Vector2Int direction = new Vector2Int((int)m_dir.x, (int)m_dir.y); // Set direction values to int and set these values to a new vector int variable
        if (m_currentNode != null) // If the entity is on a node
        {
            if (m_currentNode.GetNeighbour(direction) != null) // If the current node the entity is trying to fire into is not a wall
            {
                Gizmos.DrawCube(m_currentNode.GetNeighbour(direction).position.world, new Vector3(1, 1, 1)); // Draw a yellow square to show where the bullet is spawning.
            }
        }

        Gizmos.color = Color.cyan;

        if (m_currentNode != null)
            Gizmos.DrawRay(m_currentNode.position.world, (Vector2)positionDiff);

        Gizmos.color = (pathBlocked) ? Color.red : Color.cyan ;

        if (targetPosition != null && player != null)
            Gizmos.DrawRay(player.Position.world, -offsetVector);

        if (targetPosition != null)
            Gizmos.DrawSphere(targetPosition.world, 0.2f);

        if (Application.isPlaying)
            JUtils.DrawPath(path, App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(m_currentNode.position.world).position.world);
    }
}