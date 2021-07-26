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

    [SerializeField] private GameObject m_playerUI;
    GameObject popup;
    RectTransform rectTransform; // Add rect transform
    GameObject textObject;
    private float m_timer = 0.0f;
    private bool m_popupOn = false;
    private bool m_timing = false;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            Debug.Log("Interacted");
            App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
            ShowQuestPopup();
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
        if(m_popupOn && m_timer > 2.5f)
        {
            LeanTween.move(popup, new Vector3(rectTransform.position.x, -100.0f, rectTransform.position.z), 1.0f);
            LeanTween.move(textObject, new Vector3(rectTransform.position.x, -100.0f, rectTransform.position.z), 1.0f);

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

    private void ShowQuestPopup()
    {
        popup = new GameObject();
        popup.transform.parent = m_playerUI.transform; // Set parent
        rectTransform = popup.AddComponent<RectTransform>(); // Add rect transform
        popup.AddComponent<CanvasRenderer>(); // Add canvas renderer

        RawImage image = popup.AddComponent<RawImage>(); // Add image
        image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.localScale = new Vector3(8.0f, 2.5f, 1.0f);
        rectTransform.anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);


        textObject = new GameObject("Quest Text");
        textObject.transform.SetParent(m_playerUI.transform);

        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = "Quest - '" + App.GetModule<QuestModule>().GetActiveQuest("Test").name + "' - has been added to your Quest Log";
        text.alignment = TextAlignmentOptions.Center;
        // #TODO #Sandy ADD IN TEXT FONT

        RectTransform textTransform = textObject.GetComponent<RectTransform>();
        textTransform.anchorMin = new Vector2(0.5f, 0.0f);
        textTransform.anchorMax = new Vector2(0.5f, 0.0f);
        textTransform.pivot = new Vector2(0.5f, 0.5f);

        textTransform.localScale = new Vector3(8.0f, 2.5f, 1.0f);
        textTransform.anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);        
        textTransform.sizeDelta = new Vector2(120, 20);



        LeanTween.move(popup, new Vector3(rectTransform.position.x, 50.0f, rectTransform.position.z), 1.0f);
        LeanTween.move(textObject, new Vector3(textTransform.position.x, 50.0f, textTransform.position.z), 1.0f);

        m_popupOn = true;
        m_timing = true;
    }
}