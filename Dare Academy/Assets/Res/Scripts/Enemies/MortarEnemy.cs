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
    private int m_waitTimer = 2;

    protected override void Start()
    {
        base.Start();
        m_health = 2;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        if (PlayerInFiringRange() && !m_attack)
        {
            mortarGraphic = Instantiate(Resources.Load<GameObject>("prefabs/Entities/MortarShots"));
            mortarGraphic.transform.position = m_player.Position.world;
            centreAttack = m_player.Position.world;
            m_attack = true;
        }
        base.AnalyseStep();
    }

    public override void DamageStep()
    {
        if (m_attack)
        {
            if (m_waitTimer < 1)
            {
                m_waitTimer = 2;
                m_attack = false;
                return;
            }

            m_attackTimer += 1;
            if (m_attackTimer > 1)
            {
                GridNode centreNode = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(centreAttack);
                List<GridEntity> contents = centreNode.GetGridEntities();
                foreach (GridEntity entity in contents)
                {
                    //entity.Health -= 1;
                    Debug.Log("Hit");
                }

                for (int i = 0; i < 8; i += 2)
                {
                    GridNode node = centreNode.Neighbors[i].reference;
                    if (node != null)
                    {
                        contents = node.GetGridEntities();
                        foreach (GridEntity entity in contents)
                        {
                            //entity.Health -= 1;
                            Debug.Log("Hit");
                        }
                    }
                }

                Destroy(mortarGraphic);
                m_waitTimer -= 1;
                m_attackTimer = 0;
            }
        }
    }

    public bool PlayerInFiringRange()
    {
        return Vector3.Distance(transform.position, m_player.transform.position) <= m_attackRadius + 0.25f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
}