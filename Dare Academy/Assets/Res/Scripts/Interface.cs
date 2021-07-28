using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public abstract class Interface : MonoBehaviour
{
    private PlayerEntity m_player;
    protected bool m_playerInRange = false;

    private Sprite[] m_interactImages = new Sprite[2];

    private InputModule m_inputModule;

    public abstract void OnInteract(InputAction.CallbackContext ctx);

    public virtual void Start()
    {
        m_player = PlayerEntity.Instance;
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
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/EButton");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/AButton");
    }

    private void DeviceChanged()
    {
        if (m_playerInRange)
        {
            switch (m_inputModule.LastUsedDevice.displayName)
            {
                case "Keyboard":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Mouse":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[0];
                    break;

                case "Xbox Controller":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
                    break;

                case "Wireless Controller":
                    m_player.m_interactToolTip.GetComponentInChildren<SpriteRenderer>().sprite = m_interactImages[1];
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
}