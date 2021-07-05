using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class EightDirectionalEntity : GridEntity
{
    private int m_attackCounter = 0;
    [SerializeField] private int m_attackSpeed = 3;
    [SerializeField] private bool isFiringHorizontal = true;
    private bool isAttacking = false;

    [SerializeField] private int m_moveSpeed = 1;
    [SerializeField] private GridEntity player;
    private Vector2 m_direction = new Vector2();

    [SerializeField] private GameObject m_bulletPrefab = null;

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
        //player = GameObject.Find("Green").GetComponent<GridEntity>();
    }

    public override void AnalyseStep()
    {
        if (!isAttacking)
        {
            Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.grid, player.Position.grid);

            if (path.Length > 1)
            {
                m_direction = path[1] - path[0];
            }
            else if (path.Length == 1)
            {
                //isAttacking = true;
            }

            m_direction = new Vector2Int((int)m_direction.x, (int)m_direction.y);
        }
        else
        {
            m_direction = Vector2.zero;
        }

        SetMovementDirection(m_direction, m_moveSpeed);
    }

    public override void AttackStep()
    {
        m_attackCounter++;
        if (m_attackCounter >= m_attackSpeed)
        {
            SpawnBullets();
            m_attackCounter = 0;
        }
    }

    protected bool SpawnBullets()
    {
        if (m_bulletPrefab)
        {
            Vector2Int[] m_attackDirections;
            if (isFiringHorizontal)
            {
                m_attackDirections = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) };
            }
            else
            {
                m_attackDirections = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, -1) };
            }

            for (int j = 0; j < 4; j++)
            {
                Vector3 spawnPosition;
                if (m_currentNode.GetNeighbour(m_attackDirections[j]) != null)
                {
                    spawnPosition = m_currentNode.GetNeighbour(m_attackDirections[j]).position.world;
                    GameObject obj = GameObject.Instantiate(m_bulletPrefab, spawnPosition, Quaternion.identity);

                    if (obj)
                    {
                        BulletEntity bullet = obj.GetComponent<BulletEntity>();
                        if (bullet)
                        {
                            bullet.m_bulletDirection = m_attackDirections[j];

                            if (j == 3)
                            {
                                //isFiringHorizontal = !isFiringHorizontal;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
}