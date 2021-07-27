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
        ES.SetSelectedGameObject(m_selected);
        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
    }

    // Update is called once per frame
    private void Update()
    {
        if (App.CanvasManager.topCanvas.canvas.name == "Main Menu (Canvas)" || App.CanvasManager.topCanvas.canvas.name == "Save Select (Canvas)")
        {
            if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject)
            {
                if (m_selected != null)
                {
                    if (m_selected.name.Contains("Delete")) { }
                    //ES.currentSelectedGameObject.GetComponent<Button>().interactable = false;
                    else
                        m_selected.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);

                    if (ES.currentSelectedGameObject.name.Contains("Delete")) { }
                    //ES.currentSelectedGameObject.GetComponent<Button>().interactable = true;
                    else
                        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
                }
                m_selected = ES.currentSelectedGameObject;
            }
        }
    }

    public void HoverEnter(GameObject hoveredGo)
    {
        if (lastTouched)
            lastTouched.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);

        ES.SetSelectedGameObject(hoveredGo);
        m_selected = ES.currentSelectedGameObject;

        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
    }

    public void HoverExit()
    {
        if (m_selected != null)
        {
            LeanTween.cancel(gameObject);
            float Xsize = m_selected.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (m_selected.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
            LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
            lastTouched = m_selected;
        }
        else
        {
            //#todo #jack #adam some shit brokey
            Debug.LogWarning("m_seleced is null");
        }
    }
}