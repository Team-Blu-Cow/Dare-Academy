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
    [SerializeField] private int m_shootingWindupTime = 1;
    private bool isAttacking = false; // Boolean for if the entity is attacking or not
    private bool isWaiting = false; // Boolean for if the entity is waiting after firing a bullet

    [Header("Move attributes")]
    [SerializeField] private int moveSpeed = 1; // Set move speed to 1. Travels 1 square each step

    [Header("Resources needed")]
    [SerializeField] private GameObject m_bulletPrefab = null; // Bullet prefab for spawning bullets

    [SerializeField] private GridEntity player; // Player for finding path // WILl NEED TO BE CHANGED TO NAME OF PLAYER

    private Vector3[] path;

    [Header("Jays rework stuff")]
    [SerializeField] private Vector2Int positionDiff;

    [SerializeField] private Vector2 offsetVector;
    [SerializeField] private GridNodePosition targetPosition;
    [SerializeField] private int distance;
    [SerializeField] private int dist;
    [SerializeField] private int fearRange = 2;
    private Color m_gizmoColour;

    public enum State
    {
        MOVING,
        SHOOTING
    }

    [SerializeField] private State m_state;

    protected override void Start()
    {
        base.Start(); // Run base start function
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true); // Set flag for killable to true
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true); // Set flag for if solid to true
        player = PlayerEntity.Instance;
        m_state = State.MOVING;
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

        if (m_cooldownCounter > 0)
            m_cooldownCounter--;

        DecideState();

        switch (m_state)
        {
            case State.MOVING:
                MoveState();
                break;

            case State.SHOOTING:
                ShootingState();
                break;
        }
    }

    private void DecideState()
    {
        m_gizmoColour = Color.cyan;
        m_state = State.MOVING;

        positionDiff = player.Position.grid - this.Position.grid;

        offsetVector = new Vector2(
            (Mathf.Abs(positionDiff.x) > Mathf.Abs(positionDiff.y)) ? positionDiff.x : 0,
            (Mathf.Abs(positionDiff.y) >= Mathf.Abs(positionDiff.x)) ? positionDiff.y : 0
            );

        if (Vector2Int.Distance(this.Position.grid, player.Position.grid) <= fearRange)
        {
            m_gizmoColour = Color.red;
            m_state = State.SHOOTING;
            return;
        }

        if ((positionDiff.x == 0 || positionDiff.y == 0)
            && m_currentNode.IsPathClear(player.currentNode)
            && Vector2Int.Distance(this.Position.grid, player.Position.grid) > fearRange
            && m_cooldownCounter <= 0)
        {
            m_gizmoColour = Color.green;
            m_state = State.SHOOTING;
        }
    }

    public void MoveState()
    {
        GridNode targetNode;

        targetNode = player.currentNode.GetNeighbourDist(Vector2Int.RoundToInt(-offsetVector), Mathf.RoundToInt(offsetVector.magnitude));

        if (targetNode != null)
            targetPosition = targetNode.position;

        dist = player.currentNode.GetDistanceInDirection(Vector2Int.RoundToInt(-offsetVector));

        distance = Mathf.RoundToInt(offsetVector.magnitude);

        if (dist < distance && dist <= fearRange)
        {
            if (Mathf.Abs(offsetVector.x) > Mathf.Abs(offsetVector.y))
                offsetVector = new Vector2(0, (positionDiff.y == 0) ? 3 : positionDiff.y);
            else
                offsetVector = new Vector2((positionDiff.x == 0) ? 3 : positionDiff.x, 0);

            dist = player.currentNode.GetDistanceInDirection(Vector2Int.RoundToInt(-offsetVector));

            targetNode = player.currentNode.GetNeighbourDist(Vector2Int.RoundToInt(-offsetVector), fearRange);

            if (targetNode != null)
                targetPosition = targetNode.position;
        }

        path = new Vector3[0];
        path = App.GetModule<LevelModule>().MetaGrid.GetPathWithAvoidance(Position.world, targetPosition.world, player.Position.world, fearRange);

        Vector3 direction;

        if (path == null || path.Length <= 0)
            direction = Vector3.zero;
        else
            direction = path[0] - Position.world;

        SetMovementDirection(new Vector2(direction.x, direction.y));

        if (direction != Vector3.zero)
        {
            targetNode = m_currentNode.Neighbors[new Vector2(direction.x, direction.y)].reference;

            if (targetNode != null)
            {
                App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(targetNode, TelegraphDrawer.Type.MOVE);
            }
        }
    }

    public void ShootingState()
    {
        if (m_state != State.SHOOTING || m_cooldownCounter > 0)
            return;

        GridNode targetNode = m_currentNode.Neighbors[offsetVector].reference;

        if (targetNode != null)
        {
            App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(targetNode, TelegraphDrawer.Type.ATTACK);
        }
    }

    public override void AttackStep()
    {
        if (m_state == State.SHOOTING && m_cooldownCounter <= 0) // If the entity is attacking
        {
            m_cooldownCounter = m_attackCooldown;
            SpawnBullet(m_bulletPrefab, m_currentNode, offsetVector); // Spawn bullet in the direction the entity was moving
        }
    }

    /* this is dumb
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
    }*/

    protected override void OnDrawGizmos()
    {
        Gizmos.color = m_gizmoColour;

        if (m_currentNode != null)
            Gizmos.DrawRay(m_currentNode.position.world, (Vector2)positionDiff);

        if (targetPosition != null && player != null)
            Gizmos.DrawRay(player.Position.world, -offsetVector);

        if (targetPosition != null && m_state == State.MOVING)
            Gizmos.DrawSphere(targetPosition.world, 0.2f);

        if (Application.isPlaying)
            JUtils.DrawPath(path, App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(m_currentNode.position.world).position.world);
    }
}