using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using blu;
using TMPro;

public class ButtonSelector : MonoBehaviour
{
    private EventSystem ES;
    public GameObject m_selected;
    private GameObject lastTouched = null;

    [SerializeField] private Color m_selectedColor;
    [SerializeField] private Color m_unselectedColor;

    // Start is called before the first frame update
    private void Start()
    {
        ES = EventSystem.current;
        lastTouched = m_selected;
        ES.SetSelectedGameObject(m_selected);
        if (m_selected)
            ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().color = m_selectedColor;
    }

    // Update is called once per frame
    private void Update()
    {
        if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject)
        {
            if (m_selected != null)
            {
                if (m_selected.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text))
                    text.color = m_unselectedColor;
                else
                { }

                if (ES.currentSelectedGameObject.transform.GetChild(0).TryGetComponent(out text))
                    text.color = m_selectedColor;
                else { }
            }
            m_selected = ES.currentSelectedGameObject;
            lastTouched = m_selected;
        }
    }

    public void HoverEnter(GameObject hoveredGo)
    {
        if (lastTouched && lastTouched.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text))
            text.color = m_unselectedColor;

        ES.SetSelectedGameObject(hoveredGo);
        m_selected = ES.currentSelectedGameObject;

        if (ES.currentSelectedGameObject.transform.GetChild(0).TryGetComponent(out text))
            text.color = m_selectedColor;
    }

    public void HoverExit()
    {
        if (m_selected != null)
        {
            lastTouched = m_selected;
        }
        else
        {
            Debug.LogWarning("m_seleced is null");
        }
    }
}