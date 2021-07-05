using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class EightDirectionalEntity : GridEntity
{

    [Header("Attack attributes")]
    [SerializeField] private int m_attackSpeed = 3;
    [SerializeField] private bool isFiringHorizontal = true;
    [SerializeField] private int agroRange = 5;
    private int m_attackCounter = 0;
    private bool isAttacking = false;
    private Vector2[] telegraphPos = { new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)};


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

        Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, player.transform.position);

        if (path.Length < agroRange)
        {                
            if (m_attackCounter >= m_attackSpeed)
            {
                isAttacking = true;
                TelegraphAttack();
            }
        }
    }

    public override void AttackStep()
    {
        m_attackCounter++;
        if (isAttacking == true)
        {
            if (m_attackCounter >= m_attackSpeed)
            {
                TelegraphAttack();
                SpawnBullets();
                m_attackCounter = 0;                
            }
            isAttacking = false;
        }
    }

    private void TelegraphAttack()
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

        for (int i = 0; i < 4; i++)
        {
            if (m_currentNode.GetNeighbour(m_attackDirections[i]) != null)
            {
                telegraphPos[i] = m_currentNode.GetNeighbour(m_attackDirections[i]).position.world;
            }
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
                                isFiringHorizontal = !isFiringHorizontal;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < 4; i++)
        {
            if(telegraphPos[i] != new Vector2(0,0))
            {
                if(!isAttacking)
                {
                    Gizmos.color = new Color(0, 0, 0, 0);
                }
                else
                {
                    Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f);
                }
                
                Gizmos.DrawCube(telegraphPos[i], new Vector3(1, 1, 1));
            }
        }

        
    }
}