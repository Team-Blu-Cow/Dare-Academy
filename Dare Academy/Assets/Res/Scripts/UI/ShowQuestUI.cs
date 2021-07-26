using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using blu;

public class ShowQuestUI : MonoBehaviour
{
    [SerializeField] private GameObject m_text;
    [SerializeField] private GameObject m_tBox;
    private RectTransform rectTransform; // Add rect transform
    private float m_timer = 0.0f;
    private bool m_popupOn = false;
    private bool m_timing = false;
    private bool isGoingDown = false;


    // Update is called once per frame
    void Update()
    {
        if (m_popupOn && m_timer > 2.5f)
        {
            LeanTween.move(m_tBox, new Vector3(rectTransform.position.x, -100.0f, rectTransform.position.z), 1.0f);
            LeanTween.move(m_text, new Vector3(rectTransform.position.x, -100.0f, rectTransform.position.z), 1.0f);

            m_popupOn = false;
            m_timing = false;
            m_timer = 0.0f;

            isGoingDown = true;
        }

        if (m_timing)
            m_timer += Time.deltaTime;

        if (isGoingDown == true)
        {
            if (m_tBox.GetComponent<RectTransform>().position.y < -70.0f)
            {
                isGoingDown = false;
            }
        }
    }

    public void ShowQuestPopup()
    {
        if (m_tBox.transform.position.y < -70.0f && isGoingDown == false)
        {
            Debug.Log("Progress");
            rectTransform = m_tBox.GetComponent<RectTransform>(); // Add rect transform
            RawImage image = m_tBox.GetComponent<RawImage>(); // Add image
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

            rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(8.0f, 2.5f, 1.0f);
            rectTransform.anchoredPosition = new Vector3(0.0f, -170.0f, 0.0f);

            TextMeshProUGUI text = m_text.GetComponent<TextMeshProUGUI>();
            text.text = "Quest - '" + App.GetModule<QuestModule>().GetActiveQuest("Test").name + "' - has been added to your Quest Log";
            text.alignment = TextAlignmentOptions.Center;

            RectTransform textTransform = m_text.GetComponent<RectTransform>();

            textTransform.anchorMin = new Vector2(0.5f, 0.0f);
            textTransform.anchorMax = new Vector2(0.5f, 0.0f);
            textTransform.pivot = new Vector2(0.5f, 0.5f);

            textTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            textTransform.anchoredPosition = new Vector3(0.0f, -170.0f, 0.0f);
            textTransform.sizeDelta = new Vector2(700, 250);

            LeanTween.move(m_tBox, new Vector3(rectTransform.position.x, 50.0f, rectTransform.position.z), 1.0f);
            LeanTween.move(m_text, new Vector3(textTransform.position.x, 50.0f, textTransform.position.z), 1.0f);

            m_popupOn = true;
            m_timing = true;
        }
    }
}
