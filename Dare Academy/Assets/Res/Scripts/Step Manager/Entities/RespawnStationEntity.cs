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

    [SerializeField, HideInInspector]
    private Sprite[] m_interactImages = new Sprite[2];

    private GameObject m_interact;

    static public RespawnStationEntity CurrentRespawnStation
    {
        get { return m_currentRespawnStation; }
        set { m_currentRespawnStation = value; }
    }

    protected override void Start()
    {
        base.Start();

        m_player = PlayerEntity.Instance;

        m_interact = new GameObject("Interact");
        m_interact.transform.SetParent(transform);
        m_interact.transform.localPosition = new Vector3(0, 1, 0);
        m_interact.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        SpriteRenderer sprite = m_interact.AddComponent<SpriteRenderer>();
        sprite.sortingLayerName = "World Space UI";
    }

    protected override void OnValidate()
    {
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/Keyboard/E_Key_Dark");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/Controller/AButton");
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

            m_player.Health = m_player.MaxHealth;
            PlayerEntity.Instance.StoreHeathEnergy();
            PlayerEntity.Instance.StoreRespawnLoaction();
            App.GetModule<LevelModule>().SaveGame();

            blu.App.GetModule<blu.LevelModule>().ActiveSaveData.respawnRoomID = this.RoomIndex;

            m_currentRespawnStation = this;

            m_currentRespawnStation.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_interact.SetActive(true);
            m_playerInRange = true;
            DeviceChanged();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_interact.SetActive(false);
            m_playerInRange = false;
        }
    }

    private void DeviceChanged()
    {
        if (m_playerInRange)
        {
            if (App.GetModule<InputModule>().LastUsedDevice == null)
            {
                m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                return;
            }

            switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
            {
                case "Keyboard":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Xbox Controller":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
                    break;

                default:
                    break;
            }
        }
    }
}