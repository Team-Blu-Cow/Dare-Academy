using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil;
using blu;

public class SlideyBoi : GridEntity
{
    [SerializeField]
    private Vector2 m_direction;

    private int moveSpeed = 0;

    protected override void Start()
    {
        base.Start();
        m_health = 2;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        moveSpeed = 0;
        bool move;

        m_direction = new Vector2(1, 0);
        move = CheckLine();

        if (!move)
        {
            m_direction = new Vector2(0, 1);
            move = CheckLine();
        }
        if (!move)
        {
            m_direction = new Vector2(-1, 0);
            move = CheckLine();
        }
        if (!move)
        {
            m_direction = new Vector2(0, -1);
            move = CheckLine();
        }

        if (!move)
        {
            SetMovementDirection(Vector2.zero, moveSpeed);
        }
        else
        {
            SetMovementDirection(m_direction, moveSpeed);
            m_direction = -m_direction;
        }

        base.AnalyseStep();
    }

    private bool CheckLine()
    {
        bool found = false;
        bool move = false;

        GridNode node = m_currentNode.Neighbors[m_direction.RotationToIndex()].reference;

        while (!found)
        {
            if (node == null || node.IsTraversable() == false)
            {
                found = true;
            }
            else
            {
                foreach (GridEntity entity in node.GetGridEntities())
                {
                    if (entity.isPlayer)
                    {
                        move = true;
                    }
                }
                node = node.Neighbors[m_direction.RotationToIndex()].reference;
                moveSpeed++;
            }
        }

        return move;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 rayVec = new Vector2(Direction.x, Direction.y);
        Gizmos.DrawRay(transform.position, rayVec);
    }
}