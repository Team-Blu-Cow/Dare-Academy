using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MortarEnemy : GridEntity
{
    [SerializeField] private GridEntity m_player;
    [SerializeField] private int m_attackRadius;
    [SerializeField] private int m_mortarShotFallTime = 3;

    private GameObject mortarGraphic;

    // private Vector3 centreAttack = new Vector3();
    private bool m_attack = false;

    // private int m_attackTimer = 0;

    [SerializeField] private int m_fireRate = 2;
    private int m_cooldown = 0;

    [SerializeField, HideInInspector] private GameObject m_damageEntityPrefab;
    [SerializeField, HideInInspector] private GameObject m_shotUpPrefab;

    protected override void OnValidate()
    {
        base.OnValidate();
        m_damageEntityPrefab = Resources.Load<GameObject>("prefabs/Entities/MortarShot");
        m_shotUpPrefab = Resources.Load<GameObject>("prefabs/Entities/MortarFire");
    }

    protected override void Start()
    {
        m_player = PlayerEntity.Instance;
        base.Start();
    }

    public override void AnalyseStep()
    {
        if (isDead)
            return;

        m_cooldown--;

        m_animationController.animator.SetBool("isAsleep", !PlayerInFiringRange());

        if (PlayerInFiringRange() && m_cooldown < 0)
        {
            m_attack = true;
            m_cooldown = m_fireRate;
        }
        base.AnalyseStep();
    }

    public override void AttackStep()
    {
        if (m_attack)
        {
            m_animationController.PlayAnimation("shoot", m_stepController.stepTime * 2f);

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
                    if (node.position.grid.x == int.MaxValue)
                        continue;

                    GameObject obj = Instantiate(m_damageEntityPrefab, node.position.world, Quaternion.identity);
                    obj.GetComponent<MortarShot>().m_landTime = m_mortarShotFallTime;
                }
            }
        }
    }

    public void FireMortarShot()
    {
        GameObject obj = Instantiate(m_shotUpPrefab, transform.position, Quaternion.identity);
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

    public override void CleanUp()
    {
        m_animationController.SpawnDeathPoof(transform.position);
        base.CleanUp();
    }

    public override void OnHit(int damage, float offsetTime = 0f)
    {
        base.OnHit(damage);

        m_animationController.DamageFlash();
    }
}