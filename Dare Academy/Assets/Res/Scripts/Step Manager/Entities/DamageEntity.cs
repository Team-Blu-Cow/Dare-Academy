using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : GridEntity
{
    public enum DamageEntityType
    {
        None,
        Fire,
        Warning, // doesnt damage but im lazy
    }

    [SerializeField]
    private int m_damage = 1;

    [SerializeField]
    private DamageEntityType m_damageType;

    public DamageEntityType DamageType => m_damageType;

    public int Damage
    {
        get => m_damage;
        set => m_damage = value;
    }

    public int Countdown
    { get; set; }

    public int Linger
    { get; set; }

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
                if (entity is WyrmSection)
                    continue;

                //entity.Health -= Damage;
                entity.OnHit(Damage);
            }

            if (Linger == 0)
                Kill();

            Linger--;
        }
    }
}