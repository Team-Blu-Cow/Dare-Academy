using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class ArcherEntity : GridEntity
{

    [Header("Attack attributes")]
    [SerializeField] private int m_attackCooldown = 0;
    [SerializeField] private int agroRange = 5;
    private int m_attackCounter = 0;
    private bool isAttacking = false;
    private Vector2[] telegraphPos = { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) };

    [Header("Move attributes")]
    private Vector2 m_dir = new Vector2();
    private Vector2 m_lastDir = new Vector2();
    [SerializeField] private int moveSpeed = 1;

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

        Vector2 alignVector = player.transform.position - transform.position;
        if((alignVector.x <= 0.5f && alignVector.x >= -0.5f) || (alignVector.y <= 0.5f && alignVector.y >= -0.5f))
        {
            if((alignVector.x <= 3.0f && alignVector.x >= -3.0f) && (alignVector.y <= 3.0f && alignVector.y >= -3.0f))
            {
                if (m_attackCooldown <= 0)
                {
                    isAttacking = true;
                }
            }
        }

        if (!isAttacking && m_attackCooldown <= 0)
        {
            Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, player.transform.position);

            if (path.Length > 1)
            {
                m_dir = path[1] - path[0];
            }
            else if (path.Length < 5)
            {
                
            }
            m_dir = new Vector2Int((int)m_dir.x, (int)m_dir.y);
            m_lastDir = m_dir;
        }
        else
        {
            if (m_attackCooldown > 0)
            {
                m_attackCooldown--;
                m_dir = Vector2.zero;
            }            
        }

        SetMovementDirection(m_dir, moveSpeed);
    }

    public override void AttackStep()
    {
        if (isAttacking == true)
        {
            SpawnBullet(m_bulletPrefab, m_currentNode, m_lastDir);
            m_attackCounter = 0;
            isAttacking = false;
            m_attackCooldown = 3;
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
