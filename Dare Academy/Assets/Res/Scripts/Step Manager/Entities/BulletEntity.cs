using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtil;
using flags = GridEntityFlags.Flags;

public class BulletEntity : GridEntity
{
    public Vector2Int m_bulletDirection = Vector2Int.zero;
    public int m_damage = 1;

    private bool m_passthrough = false;

    [SerializeField] private Transform m_particleTail;
    [SerializeField] private GameObject m_ExplosionPrefab;

    protected override void Start()
    {
        base.Start();
        m_flags.SetFlags(flags.isAttack, true);
        m_health = 1;
        if (m_animationController == null)
            m_animationController = GetComponent<GridEntityAnimationController>();

        m_animationController.animator.SetFloat("DirX", m_bulletDirection.x);
        m_animationController.animator.SetFloat("DirY", m_bulletDirection.y);

        if (m_particleTail != null)
            SetTailDirection();
    }

    private void SetTailDirection()
    {
        int index = m_bulletDirection.RotationToIndex(90);

        switch (index)
        {
            case 0: // north
                transform.Rotate(new Vector3(0, 0, 180));
                break;

            case 1: // east
                transform.Rotate(new Vector3(0, 0, 90));
                break;

            case 2: // south
                transform.Rotate(new Vector3(0, 0, 0));
                break;

            case 3: // west
                transform.Rotate(new Vector3(0, 0, 270));
                break;
        }
    }

    public override bool CheckForConflict()
    {
        if (m_passthrough)
            return true;

        return base.CheckForConflict();
    }

    public override void ResolvePassThroughStep()
    {
        if (m_previousNode == null)
            return;

        List<GridEntity> entities = m_previousNode.GetGridEntities();

        // return if there are no entities
        if (entities == null || entities.Count < 1)
            return;

        // check for entities that have passed through this entity
        foreach (GridEntity entity in entities)
        {
            if (entity.Flags.IsFlagsSet(flags.isAttack))
                continue;

            if (entity.Direction == -Direction)
            {
                RemoveFromCurrentNode();
                m_currentNode = m_previousNode;
                AddToCurrentNode();
            }
        }

        return;
    }

    public override void ResolveMoveStep()
    {
    }

    public override void AnalyseStep()
    {
        if (m_bulletDirection == Vector2Int.zero)
            Kill();

        if (m_bulletDirection.x > 0)
            m_bulletDirection.x = 1;
        else if (m_bulletDirection.x < 0)
            m_bulletDirection.x = -1;

        if (m_bulletDirection.y > 0)
            m_bulletDirection.y = 1;
        else if (m_bulletDirection.y < 0)
            m_bulletDirection.y = -1;

        GridNode nextNode = m_currentNode.GetNeighbour(m_bulletDirection);
        if (nextNode == null)
            Kill();

        SetMovementDirection(m_bulletDirection);
    }

    public override void DamageStep()
    {
        TryReflectBullet();

        List<GridEntity> entities = GetEntitiesOnNode(m_currentNode);
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
        OnDeath();
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

                // animation for successful bullet reflection
                AddAnimationAction(ActionTypes.MOVE, "");
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

    public override void OnDeath()
    {
        if (m_ExplosionPrefab != null)
        {
            Vector3 pos = Vector3.Lerp(transform.position, transform.position + new Vector3(m_bulletDirection.x, (m_bulletDirection.y * 1.2f) - 0.75f, 0), 0.65f);

            Instantiate(m_ExplosionPrefab, pos, Quaternion.identity);
        }
        base.OnDeath();
    }
}