using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using blu;
using TMPro;

public class MainMenuSelector : MonoBehaviour
{
    private EventSystem ES;
    public GameObject m_selected;
    private GameObject lastTouched = null;

    [SerializeField] private float m_selectedOpacity = 255;
    [SerializeField] private float m_unselectedOpacity = 100;

    // Start is called before the first frame update
    private void Start()
    {
        ES = EventSystem.current;
        lastTouched = m_selected;
        ES.SetSelectedGameObject(m_selected);
        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
    }

    // Update is called once per frame
    private void Update()
    {
        if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject)
        {
            if (m_selected != null)
            {
                if (m_selected.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text))
                    text.color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);
                else
                { }

                if (ES.currentSelectedGameObject.transform.GetChild(0).TryGetComponent(out text))
                    text.alpha = m_selectedOpacity;
                else { }
            }
            m_selected = ES.currentSelectedGameObject;
            lastTouched = m_selected;
        }
    }

    public void HoverEnter(GameObject hoveredGo)
    {
        if (lastTouched && lastTouched.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text))
            text.color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);

        ES.SetSelectedGameObject(hoveredGo);
        m_selected = ES.currentSelectedGameObject;

        if (ES.currentSelectedGameObject.transform.GetChild(0).TryGetComponent(out text))
            text.alpha = m_selectedOpacity;
    }

    public void HoverExit()
    {
        if (m_selected != null)
        {
            lastTouched = m_selected;
        }
        else
        {
            //#todo #jack #adam some shit brokey
            Debug.LogWarning("m_seleced is null");
        }
    }
}