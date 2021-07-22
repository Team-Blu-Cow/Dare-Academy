using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using blu;

public class MainMenuSelector : MonoBehaviour
{
    private EventSystem ES;
    private GameObject m_selected = null;

    // Start is called before the first frame update
    private void Start()
    {
        ES = EventSystem.current;
        m_selected = ES.currentSelectedGameObject;
        LeanTween.moveY(gameObject, ES.firstSelectedGameObject.transform.position.y, 0.1f).setEase(LeanTweenType.easeInOutSine);
    }

    // Update is called once per frame
    private void Update()
    {
        if (App.CanvasManager.topCanvas.canvas.name == "Main Menu")
        {
            if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject && ES.currentSelectedGameObject.transform.position != Vector3.zero)
            {
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
        LeanTween.cancel(gameObject);
        float Xsize = hoveredGo.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (hoveredGo.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
        LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.moveY(gameObject, hoveredGo.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void HoverExit()
    {
        if (m_selected != null)
        {
            LeanTween.cancel(gameObject);
            float Xsize = m_selected.GetComponentInChildren<TMPro.TMP_Text>().text.Length * (m_selected.GetComponentInChildren<TMPro.TMP_Text>().fontSize / 100);
            LeanTween.scaleX(gameObject, Xsize, 0.2f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.2f).setEase(LeanTweenType.easeInOutSine);
        }
        else
        {
            //#todo #jack #adam some shit brokey
            Debug.LogWarning("m_seleced is null");
        }
    }
}