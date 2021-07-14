using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using blu;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject m_toolTip;
    public string m_questName;
    public string m_questContent;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        FollowCursor();
    }

    public void SetHeader()
    {
        m_toolTip.GetComponentsInChildren<TextMeshProUGUI>()[0].text = m_questName;
    }

    public void SetContent()
    {
        m_toolTip.GetComponentsInChildren<TextMeshProUGUI>()[1].text = m_questContent;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            m_toolTip.SetActive(true);
            SetHeader();
            SetContent();
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        m_toolTip.SetActive(false);
    }

    private void FollowCursor()
    {
        int padding = 15;

        if (m_toolTip.activeSelf)
        {
            Vector3 newPos = Mouse.current.position.ReadValue() + new Vector2(padding, padding);

            if (newPos.x + m_toolTip.GetComponentInChildren<RectTransform>().rect.width > Screen.width - padding)
            {
                newPos.x = Screen.width - m_toolTip.GetComponentInChildren<RectTransform>().rect.width - padding;
            }

            if (newPos.y + m_toolTip.GetComponentInChildren<RectTransform>().rect.height > Screen.height - padding)
            {
                newPos.y = Screen.height - m_toolTip.GetComponentInChildren<RectTransform>().rect.height - padding;
            }

            if (newPos.x < padding)
            {
                newPos.x = padding;
            }

            if (newPos.y < padding)
            {
                newPos.y = padding;
            }

            RectTransform rect = (RectTransform)m_toolTip.transform;
            rect.position = newPos;
        }
    }
}