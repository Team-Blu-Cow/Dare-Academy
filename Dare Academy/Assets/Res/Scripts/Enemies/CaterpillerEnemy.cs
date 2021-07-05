using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class CaterpillerEnemy : GridEntity
{
    [SerializeField] private GridEntity followEntity;
    public GridEntity behindEntity;
    [SerializeField] private bool head;

    private Vector2 m_dir = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    private void OnValidate()
    {
        if (followEntity.TryGetComponent(out CaterpillerEnemy caterpiller))
        {
            caterpiller.behindEntity = this;
            head = false;
        }
        else
        {
            head = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        m_health = 5;
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
    }

    public override void AnalyseStep()
    {
        Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, followEntity.Position.world);

        Vector3 dir = new Vector3();

        if (path.Length > 1)
        {
            dir = path[1] - path[0];
        }
        else
        {
            Vector2 temp = followEntity.Position.grid - Position.grid;
            dir = new Vector3(temp.x, temp.y, 0);
        }

        m_dir = new Vector2Int((int)dir.x, (int)dir.y);

        SetMovementDirection(m_dir, moveSpeed);
    }

    public override void AttackStep()
    {
        base.AttackStep();
    }

    public override void ResolvePassThroughStep()
    {
        m_currentNode.GetGridEntities();

        foreach (GridEntity entity in m_currentNode.GetGridEntities())
        {
            if (entity.isPlayer)
            {
            }
        }
        base.ResolvePassThroughStep();
    }

    public override void ResolveMoveStep()
    {
        m_currentNode.GetGridEntities();

        foreach (GridEntity entity in m_currentNode.GetGridEntities())
        {
            if (entity.isPlayer)
            {
            }
        }
        base.ResolveMoveStep();
    }
}