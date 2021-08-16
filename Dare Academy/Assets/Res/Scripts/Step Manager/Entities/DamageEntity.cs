using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

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

    public void Init(GridNode node)
    {
        m_currentNode = node;

        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_currentNode.AddEntity(this);
        m_roomIndex = m_currentNode.roomIndex;

        transform.position = Position.world;

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        m_stepController.RoomChangeEvent += RoomChange;

        if (m_roomIndex == m_stepController.m_currentRoomIndex)
        {
            m_stepController.AddEntity(this);
        }
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