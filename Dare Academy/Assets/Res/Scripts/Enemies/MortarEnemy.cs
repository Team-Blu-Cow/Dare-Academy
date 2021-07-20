using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MortarEnemy : GridEntity
{
    [SerializeField] private GridEntity m_player;
    [SerializeField] private int m_attackRadius;

    private GameObject mortarGraphic;

    private Vector3 centreAttack = new Vector3();
    private bool m_attack = false;
    private int m_attackTimer = 0;

    private int m_fireRate = 2;
    private int m_cooldown = 0;

    private GameObject m_damageEntityPrefab;

    protected override void OnValidate()
    {
        base.OnValidate();
        m_damageEntityPrefab = Resources.Load<GameObject>("prefabs/Entities/DamageEntity");
    }

    protected override void Start()
    {
        m_health = 2;
        m_player = FindObjectOfType<PlayerEntity>();
        base.Start();
    }

    public override void AnalyseStep()
    {
        m_cooldown--;

        if (PlayerInFiringRange() && m_cooldown < 0)
        {
            m_attack = true;
            m_cooldown = m_fireRate;
        }
        base.AnalyseStep();
    }

    public override void DamageStep()
    {
        if (m_attack)
        {
            m_attack = false;

            List<GridNode> nodes = new List<GridNode>();
            nodes.Add(m_player.currentNode);

            for (int i = 0; i < 8; i += 2)
            {
                nodes.Add(m_player.currentNode.Neighbors[i].reference);
            }

            foreach (var node in nodes)
            {
                if (node != null && node != currentNode)
                {
                    GameObject obj = GameObject.Instantiate(m_damageEntityPrefab, node.position.world, Quaternion.identity);
                    obj.GetComponent<DamageEntity>().Countdown = 2;
                }
            }
        }
    }

    private bool PlayerInFiringRange()
    {
        if (m_player.currentNode == null)
            return false;

        return Vector3.Distance(transform.position, m_player.Position.world) <= m_attackRadius + 0.25f;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
}