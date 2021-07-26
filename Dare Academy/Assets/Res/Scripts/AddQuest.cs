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


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            Debug.Log("Interacted");
            App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
            m_questPopup.ShowQuestPopup();
        }
    }

    public bool PlayerInRange(Transform transform)
    {
        return false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<InputModule>().PlayerController.Player.Interact.started += OnInteract;
        m_player = PlayerEntity.Instance;
        m_questPopup = GameObject.Find("PlayerUI").GetComponent<ShowQuestUI>();
        App.GetModule<InputModule>().LastDeviceChanged += DeviceChanged;
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().LastDeviceChanged -= DeviceChanged;
        App.GetModule<InputModule>().PlayerController.Player.Interact.started -= OnInteract;
    }

    private void OnValidate()
    {
        m_interactImages[0] = Resources.Load<Sprite>("GFX/ButtonImages/EButton");
        m_interactImages[1] = Resources.Load<Sprite>("GFX/ButtonImages/AButton");
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

    private void Update()
    {

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