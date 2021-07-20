using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : GridEntity
{
    private int m_damage = 1;
    private int m_count = 0;

    public int Damage
    {
        get => m_damage;
        set => m_damage = value;
    }

    public int Countdown
    {
        get => m_count;
        set => m_count = value;
    }

    public override void AnalyseStep()
    {
        SetMovementDirection(Vector2Int.zero, 0);
    }

    public override void DamageStep()
    {
        Countdown--;

        if (Countdown < 0)
        {
            List<GridEntity> entities = GetEntitiesOnNode(m_currentNode);
            foreach (var entity in entities)
            {
                entity.Health -= Damage;
            }
            Kill();
        }
    }
}