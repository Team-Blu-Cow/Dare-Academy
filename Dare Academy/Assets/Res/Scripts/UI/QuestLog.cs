using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using blu;
using TMPro;

public class QuestLog : MonoBehaviour, IPointerDownHandler
{
    private List<GameObject> m_instantiatedQuests = new List<GameObject>();
    private bool m_paused = false;
    public bool Open { get { return m_paused; } set { m_paused = value; } }

    private int m_selectedIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<InputModule>().SystemController.UI.Map.performed += ToggleQuests;

        App.GetModule<InputModule>().SystemController.MapControlls.ScrollQuests.started += ScrollQuest;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        App.GetModule<InputModule>().SystemController.UI.Map.performed -= ToggleQuests;

        App.GetModule<InputModule>().SystemController.MapControlls.ScrollQuests.started -= ScrollQuest;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void ScrollQuest(InputAction.CallbackContext ctx)
    {
        if (m_instantiatedQuests.Count > 0)
        {
            float dir = ctx.ReadValue<float>();
            List<Quest> activeQuests = App.GetModule<QuestModule>().ActiveQuests;

            ClearContent(m_instantiatedQuests[m_selectedIndex]);

            if (dir < 0 && m_selectedIndex < activeQuests.Count - 1)
            {
                m_selectedIndex++;
            }
            else if (dir > 0 && m_selectedIndex > 0)
            {
                m_selectedIndex--;
            }

            SetContent(m_instantiatedQuests[m_selectedIndex], activeQuests[m_selectedIndex].activeDescription);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }

    private void ToggleQuests(InputAction.CallbackContext ctx)
    {
        if (m_paused)
        {
            CloseQuests();
            m_paused = false;
        }
        else
        {
            UpdateQuests();

            m_paused = true;
        }
    }

    private void CloseQuests()
    {
        foreach (GameObject Go in m_instantiatedQuests)
        {
            Destroy(Go);
        }
        m_instantiatedQuests.Clear();

        gameObject.SetActive(false);
    }

    private void UpdateQuests()
    {
        CloseQuests();

        int i = 0;
        foreach (Quest quest in App.GetModule<QuestModule>().ActiveQuests)
        {
            GameObject questPrefab = Resources.Load<GameObject>("prefabs/UI prefabs/Quest");

            GameObject questGo = Instantiate(questPrefab, transform);
            m_instantiatedQuests.Add(questGo);

            Toggle toggle = m_instantiatedQuests[i].GetComponentInChildren<Toggle>();

            SetHeader(questGo, quest.name);
            if (i == m_selectedIndex)
                SetContent(questGo, quest.activeDescription);
            else
                ClearContent(questGo);

            toggle.isOn = quest.showMarker;

            int localInt = i;
            toggle.onValueChanged.AddListener(delegate { ToggleMarker(localInt); });

            i++;
        }

        if (m_instantiatedQuests.Count > 0)
        {
            for (int j = 0; j < m_instantiatedQuests.Count; j++)
            {
                Toggle toggle = m_instantiatedQuests[j].GetComponentInChildren<Toggle>();

                Navigation nav = toggle.navigation;
                nav.mode = Navigation.Mode.Explicit;

                if (j + 1 < m_instantiatedQuests.Count)
                    nav.selectOnDown = m_instantiatedQuests[j + 1].GetComponentInChildren<Toggle>();
                else
                    nav.selectOnDown = m_instantiatedQuests[0].GetComponentInChildren<Toggle>();

                if (j - 1 > 0)
                    nav.selectOnUp = m_instantiatedQuests[j - 1].GetComponentInChildren<Toggle>();
                else
                    nav.selectOnUp = m_instantiatedQuests[m_instantiatedQuests.Count - 1].GetComponentInChildren<Toggle>();

                toggle.navigation = nav;
            }

            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(m_instantiatedQuests[m_selectedIndex].GetComponentInChildren<Toggle>().gameObject);
        }
    }

    public void ToggleMarker(int in_i)
    {
        App.GetModule<QuestModule>().ActiveQuests[in_i].showMarker = !App.GetModule<QuestModule>().ActiveQuests[in_i].showMarker;
        FindObjectOfType<MiniMapGen>().DrawQuestMarker(App.GetModule<LevelModule>().CurrentRoom);
    }

    private void SetHeader(GameObject target, string header)
    {
        target.GetComponentsInChildren<TextMeshProUGUI>()[0].text = header;
    }

    private void SetContent(GameObject target, string content)
    {
        target.GetComponentsInChildren<TextMeshProUGUI>()[1].text = content;
        target.GetComponent<VerticalLayoutGroup>().spacing = 20;
    }

    private void ClearContent(GameObject target)
    {
        target.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "";
        target.GetComponent<VerticalLayoutGroup>().spacing = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RaycastResult ray = eventData.pointerCurrentRaycast;

        if (ray.gameObject.name == "Quest(Clone)")
        {
            ClearContent(m_instantiatedQuests[m_selectedIndex]);

            for (int i = 0; i < ray.gameObject.transform.parent.childCount; i++)
            {
                if (ray.gameObject.transform.parent.GetChild(i) == ray.gameObject.transform)
                {
                    m_selectedIndex = i;
                    break;
                }
            }

            SetContent(m_instantiatedQuests[m_selectedIndex], App.GetModule<QuestModule>().ActiveQuests[m_selectedIndex].activeDescription);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }
}