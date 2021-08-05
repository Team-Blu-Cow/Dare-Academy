using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : GridEntity
{
    private int m_damage = 1;

    [SerializeField]
    readonly public bool IsFire;

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
                //entity.Health -= Damage;
                entity.OnHit(Damage);
            }

            if (Linger == 0)
                Kill();

            Linger--;
        }
    }
}