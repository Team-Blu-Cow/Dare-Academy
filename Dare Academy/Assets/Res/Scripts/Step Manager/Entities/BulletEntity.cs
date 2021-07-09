using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;

public class BulletEntity : GridEntity
{
    public Vector2Int m_bulletDirection = Vector2Int.zero;
    public int m_damage = 1;

    private bool m_passthrough = false;

    protected override void Start()
    {
        base.Start();
        m_flags.SetFlags(flags.isAttack, true);
        m_health = 1;
        m_animationController = GetComponent<GridEntityAnimationController>();
    }

    public override bool CheckForConflict()
    {
        if (m_passthrough)
            return true;

        return base.CheckForConflict();
    }

    public override void ResolvePassThroughStep()
    {
        if (CheckForPassThrough())
        {
            if (m_currentNode != null && m_previousNode != null)
            {
                m_passthrough = true;
                RemoveFromCurrentNode();
                m_currentNode = m_previousNode;
                AddToCurrentNode();

                TryReflectBullet();
            }
        }
    }

    public override void ResolveMoveStep()
    {
    }

    public override void AnalyseStep()
    {
        if (m_bulletDirection == Vector2Int.zero)
            Kill();

        GridNode nextNode = m_currentNode.GetNeighbour(m_bulletDirection);
        if (nextNode == null)
            Kill();

        SetMovementDirection(m_bulletDirection);
    }

    public override void DamageStep()
    {
        TryReflectBullet();

        List<GridEntity> entities =  GetEntitiesOnNode(m_currentNode);
        foreach (GridEntity entity in entities)
        {
            entity.Health -= m_damage;
            Kill();
        }
    }

    public override void EndStep()
    {
        m_passthrough = false;
        base.EndStep();
    }

    public override void RoomChange()
    {
        CleanUp();
    }

    protected void TryReflectBullet()
    {
        List<GridEntity> entities = GetEntitiesOnNode(m_currentNode);
        if (entities.Count == 1) // if not wait until next loop
        {
            if (entities[0].ShouldReflectBullet(m_bulletDirection))
            {
                m_bulletDirection = -m_bulletDirection;
                GridNode node = m_currentNode.GetNeighbour(m_bulletDirection);

                RemoveFromCurrentNode();
                m_currentNode = node;
                AddToCurrentNode();
                m_previousNode = null;
                // TODO @Jay - animation for successful bullet reflection
            }
            m_passthrough = false;
        }
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Vector2 rayVec = new Vector2(m_bulletDirection.x, m_bulletDirection.y);
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}