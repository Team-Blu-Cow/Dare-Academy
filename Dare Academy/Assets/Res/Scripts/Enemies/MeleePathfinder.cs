using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class MeleePathfinder : GridEntity
{
    private int m_waitCounter = 0;
    private bool m_attack = false;
    private Vector2 m_attackDirection = new Vector2();

    private Vector2 m_dir = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    [SerializeField] private GridEntity player;

    protected override void Start()
    {
        base.Start();
        m_health = 5;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        if (!m_attack)
        {
            Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.grid, player.Position.grid);

            Vector3 dir = new Vector3();

            if (path.Length > 1)
                dir = path[1] - path[0];
            else if (path.Length == 1)
            {
                m_attackDirection = player.Position.grid - Position.grid;
                m_attack = true;
            }

            m_dir = new Vector2Int((int)dir.x, (int)dir.y);
        }
        else
        {
            m_dir = Vector2.zero;
        }

        SetMovementDirection(m_dir, moveSpeed);
    }

    public override void AttackStep()
    {
        if (m_attack)
        {
            m_waitCounter += 1;

            if (m_waitCounter > 1)
            {
                GridNode node = m_currentNode.Neighbors[m_attackDirection.RotationToIndex()].reference;
                if (node != null)
                {
                    List<GridEntity> contents = node.GetGridEntities();
                    foreach (GridEntity entity in contents)
                    {
                        if (entity.isPlayer)
                        {
                            Debug.Log("Hit");
                            break;
                        }
                    }
                }
                m_waitCounter = 0;
                m_attack = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, m_dir);
    }
}