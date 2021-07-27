using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using UnityEngine.UI;

using TMPro;
using blu;

public class AddQuest : MonoBehaviour, IInteractable
{
    private PlayerEntity m_player;
    private bool m_playerInRange = false;

    private Sprite[] m_interactImages = new Sprite[2];

    private ShowQuestUI m_questPopup;

    private InputModule m_inputModule;

    [SerializeField] private Quest m_quest;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            if (m_quest)
            {
                if (App.GetModule<QuestModule>().AddQuest(m_quest))
                    m_questPopup.ShowQuestPopup();
            }
            else
            {
                Debug.LogWarning("[AddQuest.cs] m_quest was null");
            }
        }
    }

    private void Start()
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

    private void OnValidate()
    {
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/EButton");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/AButton");
        m_questPopup = GameObject.FindObjectOfType<ShowQuestUI>();
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