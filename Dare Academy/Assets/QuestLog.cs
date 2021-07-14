using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using blu;
using TMPro;

public class QuestLog : MonoBehaviour
{
    private List<GameObject> m_instantiatedQuests = new List<GameObject>();
    private bool m_paused = false;
    private int m_selectedIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Open.performed += _ => ToggleQuests();
        App.GetModule<InputModule>().SystemController.MapControlls.ScrollQuests.started += ctx => ScrollQuest(ctx);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void ScrollQuest(InputAction.CallbackContext ctx)
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

        enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        enabled = true;
        //UpdateQuests();
    }

    private void ToggleQuests()
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
            GameObject questPrefab = Resources.Load<GameObject>("prefabs/Quest");

            SetHeader(questPrefab, quest.name);
            if (i == m_selectedIndex)
                SetContent(questPrefab, quest.activeDescription);
            else
                ClearContent(questPrefab);

            m_instantiatedQuests.Add(Instantiate(questPrefab, transform));

            int localInt = i;
            m_instantiatedQuests[i].GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate { ToggleMarker(localInt); });
            i++;
        }
        gameObject.SetActive(true);
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
        target.GetComponentsInChildren<TextMeshProUGUI>()[2].text = content;
        target.GetComponent<VerticalLayoutGroup>().spacing = 20;
    }

    private void ClearContent(GameObject target)
    {
        target.GetComponentsInChildren<TextMeshProUGUI>()[2].text = "";
        target.GetComponent<VerticalLayoutGroup>().spacing = 0;
    }
}