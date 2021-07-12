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
               SpawnBullet(m_bulletPrefab, m_currentNode, m_attackDirection);
            }

            m_attackCounter = 0;
        }
    }
}