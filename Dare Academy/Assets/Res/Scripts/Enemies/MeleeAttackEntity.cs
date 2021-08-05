using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MeleeAttackEntity : GridEntity
{
    public int m_damage = 1;
    public float m_damageTimeOffset = 0f;

    public GameObject m_vfxPrefab;

    private List<GridEntity> entities;

    public void Init(GridNode node, GameObject prefab = null)
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

        if(prefab != null)
        {
            m_vfxPrefab = prefab;
        }
    }

    public override void DamageStep()
    {
        //Instantiate(m_vfxPrefab, m_currentNode.position.world + new Vector3(0, 0.25f, 1), Quaternion.identity);

        entities = m_currentNode.GetGridEntities();

        foreach (var e in entities)
        {
            if(e.Flags.IsFlagsSet(GridEntityFlags.Flags.isKillable))
            {
                //e.Health -= m_damage;
                e.OnHit(m_damage, m_damageTimeOffset);
            }
        }
    }

    public override void DrawStep()
    {
        
    }

    public override void EndStep()
    {
        //RemoveFromCurrentNode();
        OnDeath();
    }

    public override void RoomChange()
    {
        OnDeath();
    }
}
