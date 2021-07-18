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
        LeanTween.moveY(gameObject, ES.firstSelectedGameObject.transform.position.y, 0.1f);

    }

    // Update is called once per frame
    private void Update()
    {
        if (App.CanvasManager.topCanvas.canvas.name == "Main Menu (Canvas)")
        {
            if (ES.currentSelectedGameObject != null && m_selected != ES.currentSelectedGameObject && ES.currentSelectedGameObject.transform.position != Vector3.zero)
            {
                m_selected = ES.currentSelectedGameObject;

                LeanTween.cancel(gameObject);
                LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.5f);
            }
        }
    }

    public void HoverEnter(GameObject hoveredGo)
    {
        LeanTween.cancel(gameObject);

        LeanTween.moveY(gameObject, hoveredGo.transform.position.y, 0.5f);
    }

    public void HoverExit()
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveY(gameObject, m_selected.transform.position.y, 0.5f);
    }
}