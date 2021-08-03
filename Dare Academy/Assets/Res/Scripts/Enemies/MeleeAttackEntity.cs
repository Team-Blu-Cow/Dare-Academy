using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MeleeAttackEntity : GridEntity
{
    public int m_damage = 1;

    public GameObject m_vfxPrefab;

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

        foreach (var e in m_currentNode.GetGridEntities())
        {
            if(e.Flags.IsFlagsSet(GridEntityFlags.Flags.isKillable))
            {
                e.Health -= m_damage;
            }
        }
    }

    public override void EndStep()
    {
        OnDeath();
    }

    public override void RoomChange()
    {
        OnDeath();
    }
}
