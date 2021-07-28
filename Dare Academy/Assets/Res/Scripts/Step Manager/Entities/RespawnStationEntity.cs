using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using flags = GridEntityFlags.Flags;
using blu;

public class RespawnStationEntity : GridEntity, IInteractable
{
    [SerializeField] private Vector2Int m_respawnLocation = Vector2Int.zero;

    static private RespawnStationEntity m_currentRespawnStation = null;

    private bool m_playerInRange;
    private PlayerEntity m_player;

    private Sprite[] m_interactImages = new Sprite[2];

    static public RespawnStationEntity CurrentRespawnStation
    {
        get { return m_currentRespawnStation; }
        set { m_currentRespawnStation = value; }
    }

    protected override void Start()
    {
        base.Start();

        m_player = PlayerEntity.Instance;
    }

    private void OnValidate()
    {
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/EButton");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/AButton");
    }

    private void OnEnable()
    {
        App.GetModule<InputModule>().PlayerController.Player.Interact.started += OnInteract;
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().PlayerController.Player.Interact.started -= OnInteract;
    }

    private void Update()
    {
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

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            if (m_currentRespawnStation != null)
                m_currentRespawnStation.GetComponent<SpriteRenderer>().color = Color.white;

            blu.App.GetModule<blu.LevelModule>().ActiveSaveData.respawnRoomID = this.RoomIndex;

            m_currentRespawnStation = this;

            m_currentRespawnStation.GetComponent<SpriteRenderer>().color = Color.black;

            m_player.Health = m_player.MaxHealth;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.m_interactToolTip.SetActive(true);
            m_playerInRange = true;
            DeviceChanged();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.m_interactToolTip.SetActive(false);
            m_playerInRange = false;
        }
    }

    private void DeviceChanged()
    {
        if (m_playerInRange)
        {
            switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
            {
                case "Keyboard":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Xbox Controller":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
                    break;

                default:
                    break;
            }
        }
    }
}