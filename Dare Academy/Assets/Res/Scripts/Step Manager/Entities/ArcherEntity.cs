using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class ArcherEntity : GridEntity
{

    [Header("Attack attributes")]
    [SerializeField] private int m_attackCooldown = 5;
    [SerializeField] private int agroRange = 5;
    [SerializeField] private int m_cooldownCounter = 0;
    private bool isAttacking = false;
    private bool isWaiting = false;

    [Header("Move attributes")] 
    [SerializeField] private int moveSpeed = 1;
    private Vector2 m_dir = new Vector2();

    [Header("Resources needed")]
    [SerializeField] private GameObject m_bulletPrefab = null;
    [SerializeField] private GridEntity player;

    protected override void Start()
    {
        base.Start();
        m_health = 5;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public void OnValidate()
    {
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
        player = GameObject.Find("Green").GetComponent<PlayerEntity>();
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();

        Vector2 distanceVector = player.transform.position - transform.position;
        if ((distanceVector.x <= 0.5f && distanceVector.x >= -0.5f) || (distanceVector.y <= 0.5f && distanceVector.y >= -0.5f))
        {
            if ((distanceVector.x <= 4.0f && distanceVector.x >= -4.0f) && (distanceVector.y <= 4.0f && distanceVector.y >= -4.0f))
            {
                if (!CheckPlayerIsBehindWall(m_dir))
                {
                    if (m_cooldownCounter <= 0)
                    {
                        m_cooldownCounter = m_attackCooldown;
                        isAttacking = true;
                    }
                }
            }
        }

        Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, player.transform.position);

        if (!isAttacking && !isWaiting)
        {
            if (path.Length <= agroRange)
            {
                if (path.Length > 1)
                {
                    m_dir = path[1] - path[0];
                }

                m_dir = new Vector2Int((int)m_dir.x, (int)m_dir.y);
                SetMovementDirection(m_dir, moveSpeed);
            }
        }
        else
        {
           isWaiting = false;
           SetMovementDirection(Vector2.zero, moveSpeed);         
        }

        if (m_cooldownCounter > 0)
        {
            m_cooldownCounter--;
        }

    }

    public override void AttackStep()
    {
        if (isAttacking == true)
        {
            SpawnBullet(m_bulletPrefab, m_currentNode, m_dir);
            isAttacking = false;
            isWaiting = true;
        }
    }

    private bool CheckPlayerIsBehindWall(Vector2 direction)
    {
        Vector2Int m_direction = new Vector2Int((int)direction.x, (int)direction.y);

        if (m_currentNode.GetNeighbour(m_direction) == null)
        {
            return true;
        }
        else if (m_currentNode.GetNeighbour(m_direction).GetNeighbour(m_direction) == null)
        {
            return true;
        }
        else if (m_currentNode.GetNeighbour(m_direction).GetNeighbour(m_direction).GetNeighbour(m_direction) == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!isAttacking)
        {
            Gizmos.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f);
        }

        Vector2Int direction = new Vector2Int((int)m_dir.x, (int)m_dir.y);
        if (m_currentNode != null)
        {
            if (m_currentNode.GetNeighbour(direction) != null)
            {
                Gizmos.DrawCube(m_currentNode.GetNeighbour(direction).position.world, new Vector3(1, 1, 1));
            }
        }
    }

}
