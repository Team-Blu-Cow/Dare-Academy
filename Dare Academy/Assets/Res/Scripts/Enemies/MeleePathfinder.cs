using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class MeleePathfinder : GridEntity
{
    private int m_attackCounter = 0;
    private int m_waitCounter = 1;
    private bool m_attack = false;
    private Vector2 m_attackDirection = new Vector2();

    private Vector2 m_dir = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    protected override void Start()
    {
        base.Start();
        m_health = 5;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        PlayerEntity player = PlayerEntity.Instance;
        if (player.currentNode == null)
            return;

        if (!m_attack)
        {
            Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, player.Position.world);

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

        base.AnalyseStep();
    }

    public override void AttackStep()
    {
        if (m_attack)
        {
            if (m_waitCounter < 1)
            {
                m_waitCounter = 1;
                m_attack = false;
                return;
            }

            m_attackCounter += 1;

            if (m_attackCounter > 0)
            {
                GridNode node = m_currentNode.Neighbors[m_attackDirection.RotationToIndex()].reference;
                if (node != null)
                {
                    List<GridEntity> contents = node.GetGridEntities();
                    foreach (GridEntity entity in contents)
                    {
                        //entity.Health -= 1;
                        Debug.Log("Hit");
                    }
                }
                m_attackCounter = 0;
                m_waitCounter -= 1;
            }
        }
    }
}