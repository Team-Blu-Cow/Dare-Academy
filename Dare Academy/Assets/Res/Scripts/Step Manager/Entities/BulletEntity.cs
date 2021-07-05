using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using JUtil;
using JUtil.Grids;

public class BulletEntity : GridEntity
{
    public Vector2 m_bulletDirection = Vector2.zero;
    public int m_damage = 1;

    private bool m_spawnedThisStep = true;

    protected override void Start()
    {
        base.Start();
        m_flags.SetFlags(flags.isAttack, true);
        m_health = 1;
    }

    public override void ResolvePassThroughStep()
    {
        if (CheckForPassThrough())
        {
            if (m_currentNode != null && m_previousNode != null)
            {
                m_currentNode.RemoveEntity(this);
                m_previousNode = m_currentNode;
                m_currentNode.AddEntity(this);
            }
        }
    }

    public override void ResolveMoveStep()
    {
        return;
    }

    public override void AnalyseStep()
    {
        if (m_bulletDirection == Vector2.zero)
            Kill();

        GridNode nextNode = m_currentNode.Neighbors[m_bulletDirection.RotationToIndex()].reference;
        if (nextNode == null)
            Kill();

        SetMovementDirection(m_bulletDirection);
    }

    public override void DamageStep()
    {
        List<GridEntity> entities =  GetEntitiesOnNode(m_currentNode);
        foreach (GridEntity entity in entities)
        {
            entity.Health -= m_damage;
            Kill();
        }
    }

    public override void RoomChange()
    {
        CleanUp();
    }
}