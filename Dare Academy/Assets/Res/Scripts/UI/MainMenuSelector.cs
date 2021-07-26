using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using blu;
using TMPro;

public class MainMenuSelector : MonoBehaviour
{
    private EventSystem ES;
    private GameObject m_selected = null;
    private GameObject lastTouched = null;

    [SerializeField] private float m_selectedOpacity = 255;
    [SerializeField] private float m_unselectedOpacity = 100;

    // Start is called before the first frame update
    private void Start()
    {
        ES = EventSystem.current;
        ES.SetSelectedGameObject(m_selected);
        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
        LeanTween.cancel(gameObject);
        float Xsize = m_selected.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (m_selected.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
        LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    // Update is called once per frame
    private void Update()
    {
        if (App.CanvasManager.topCanvas.canvas.name == "Main Menu (Canvas)")
        {
            if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject)
            {
                if (m_selected != null)
                {
                    m_selected.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);
                    ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
                }
                m_selected = ES.currentSelectedGameObject;
                LeanTween.cancel(gameObject);
                float Xsize = m_selected.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (m_selected.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
                LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
            }
        }
    }

    public void HoverEnter(GameObject hoveredGo)
    {
        if (lastTouched)
            lastTouched.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, (byte)m_unselectedOpacity);

        ES.SetSelectedGameObject(hoveredGo);
        m_selected = ES.currentSelectedGameObject;
        LeanTween.cancel(gameObject);
        float Xsize = m_selected.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (m_selected.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
        LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);

        ES.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().alpha = m_selectedOpacity;
        //LeanTween.cancel(gameObject);
        //float Xsize = hoveredGo.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (hoveredGo.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
        //LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
        //LeanTween.moveY(gameObject, hoveredGo.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
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