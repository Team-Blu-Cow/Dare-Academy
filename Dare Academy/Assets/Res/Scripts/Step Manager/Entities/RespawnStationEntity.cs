using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;

public class RespawnStationEntity : GridEntity
{
    [SerializeField] private Vector2Int m_respawnLocation = Vector2Int.zero;

    static private RespawnStationEntity m_currentRespawnStation = null;

    static public RespawnStationEntity CurrentRespawnStation
    {
        get { return m_currentRespawnStation; }
        set { m_currentRespawnStation = value; }
    }

    private void Update()
    {
        Color color = new Color(0,0,0,1);

        if (m_currentRespawnStation == this)
        {
            color = new Color(1, 1, 1, 1);
        }

        GetComponent<SpriteRenderer>().color = color;
    }

    public override void EndStep()
    {
        base.EndStep();

        bool nextToPlayer = false;

        for (int i = 0; i < 8; i++)
        {
            GridNode neighbour = m_currentNode.Neighbors[i].reference;
            if (neighbour != null)
            {
                List<GridEntity> entities = neighbour.GetGridEntities();
                foreach (GridEntity entity in entities)
                {
                    if (entity.Flags.IsFlagsSet(flags.isPlayer))
                    {
                        nextToPlayer = true;
                        break;
                    }
                }
            }
        }

        if (nextToPlayer)
        {
            blu.App.GetModule<blu.LevelModule>().ActiveSaveSata.respawnRoomID = this.RoomIndex;
            blu.App.GetModule<blu.LevelModule>().UpdateCheckpoint();

            m_currentRespawnStation = this;
        }
    }

    public GridNode RespawnLocation()
    {
        GridNode node = m_currentNode.GetNodeRelative(m_respawnLocation);
        if (node != null)
        {
            return node;
        }

        Debug.LogWarning($"Respawn point invalid, attempting to spawn in neighboring node [Room ID = {RoomIndex}]");

        for (int i = 0; i < 8; i++)
        {
            GridNode neighbour = m_currentNode.Neighbors[i].reference;
            if (neighbour != null)
            {
                List<GridEntity> entities = neighbour.GetGridEntities();
                if (entities.Count == 0)
                {
                    node = neighbour;
                    break;
                }
            }
        }
        if (node != null)
        {
            return node;
        }

        Debug.LogWarning($"Could not find fallback respawn loaction [Room ID = {RoomIndex}]");

        return null;
    }
}