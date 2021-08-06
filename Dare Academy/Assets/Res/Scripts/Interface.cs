using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using blu;

public abstract class Interface : MonoBehaviour
{
    protected PlayerEntity m_player;
    protected bool m_playerInRange = false;

    [SerializeField, HideInInspector]
    private Sprite[] m_interactImages = new Sprite[2];

    private InputModule m_inputModule;

    private GameObject m_interact;

    public abstract void OnInteract(InputAction.CallbackContext ctx);

    public virtual void Start()
    {
        m_player = PlayerEntity.Instance;

        m_interact = new GameObject("Interact");
        m_interact.transform.SetParent(transform);
        m_interact.transform.localPosition = new Vector3(0, 1, 0);
        m_interact.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        SpriteRenderer sprite = m_interact.AddComponent<SpriteRenderer>();
        sprite.sortingLayerName = "World Space UI";
    }

    private void OnEnable()
    {
        if (m_inputModule == null)
            m_inputModule = App.GetModule<InputModule>();

        m_inputModule.PlayerController.Player.Interact.started += OnInteract;
        m_inputModule.LastDeviceChanged += DeviceChanged;
    }

    private void OnDisable()
    {
        m_inputModule.LastDeviceChanged -= DeviceChanged;
        m_inputModule.PlayerController.Player.Interact.started -= OnInteract;
    }

    public virtual void OnValidate()
    {
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/Keyboard/E_Key_Dark");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/Controller/AButton");
    }

    private void DeviceChanged()
    {
        if (m_playerInRange)
        {
            switch (m_inputModule.LastUsedDevice.displayName)
            {
                case "Keyboard":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Mouse":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Xbox Controller":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
                    break;

                case "Wireless Controller":
                    m_interact.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
                    break;

                default:

                    Debug.LogWarning($"[AddQuest.cs] unknown input device type [InputType = {m_inputModule.LastUsedDevice.displayName}]");
                    break;
            }
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
}