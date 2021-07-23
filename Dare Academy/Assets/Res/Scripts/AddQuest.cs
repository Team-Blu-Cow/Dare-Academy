using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using blu;

public class AddQuest : MonoBehaviour, IInteractable
{
    private PlayerEntity m_player;
    private bool m_playerInRange = false;

    [SerializeField] private GameObject m_playerUI;
    GameObject popup;
    RectTransform rectTransform; // Add rect transform
    private float m_timer = 0.0f;
    private bool m_popupOn = false;
    private bool m_timing = false;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            Debug.Log("Interacted");
            ShowQuestPopup();
            App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
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
    }

    private void Update()
    {
        if(m_popupOn && m_timer > 4.0f)
        {
            LeanTween.move(popup, new Vector3(rectTransform.position.x, -50.0f, rectTransform.position.z), 1.0f);
        }

        if (m_timing)
            m_timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.m_interactToolTip.SetActive(true);
            m_playerInRange = true;
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

    private void ShowQuestPopup()
    {
        popup = new GameObject();
        popup.transform.parent = m_playerUI.transform; // Set parent
        rectTransform = popup.AddComponent<RectTransform>(); // Add rect transform
        popup.AddComponent<CanvasRenderer>(); // Add canvas renderer
        RawImage image = popup.AddComponent<RawImage>(); // Add image

        rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.localScale = new Vector3(8.0f, 2.5f, 1.0f);
        rectTransform.anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);
        LeanTween.move(popup, new Vector3(rectTransform.position.x, 50.0f, rectTransform.position.z), 1.0f);

        m_popupOn = true;
        m_timing = true;
    }
}