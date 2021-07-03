using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil;

public class TurretEntity : GridEntity
{
    private int m_attackCounter = 0;
    [SerializeField] private int m_attackSpeed = 3;
    [SerializeField] private Vector2 m_attackDirection = Vector2.zero;

    [SerializeField] private GameObject m_bulletPrefab = null;

    protected override void Start()
    {
        base.Start();
        m_health = 5;
    }

    public void OnValidate()
    {
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet");
    }

    public override void AnalyseStep()
    {
    }

    public override void AttackStep()
    {
        m_attackCounter++;
        if (m_attackCounter >= m_attackSpeed)
        {
            if (m_attackDirection != Vector2.zero)
            {
                SpawnBullet();
            }

            m_attackCounter = 0;
        }
    }

    protected bool SpawnBullet()
    {
        if (m_bulletPrefab)
        {
            Vector3 spawnPosition = m_currentNode.Neighbors[m_attackDirection.RotationToIndex()].reference.position.world;

            GameObject obj = GameObject.Instantiate(m_bulletPrefab,spawnPosition, Quaternion.identity) ;
            if (obj)
            {
                BulletEntity bullet = obj.GetComponent<BulletEntity>();
                if (bullet)
                {
                    bullet.m_bulletDirection = m_attackDirection;
                    return true;
                }
            }
        }

        return false;
    }
}