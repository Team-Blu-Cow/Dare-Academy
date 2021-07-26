using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using blu;

public class ShowQuestUI : MonoBehaviour
{
    [SerializeField] private GameObject m_text; // Game object for the quest text UI object
    [SerializeField] private GameObject m_tBox; // Game object for the transparent box behind the text
    private RectTransform rectTransform; // Rect transform of the transparent box
    private float m_timer = 0.0f; // Timer for how long the quest popup should remain on screen
    private bool m_popupOn = false; // Bool for if the popup is on screen
    private bool m_timing = false; // Bool for if the timer should be timing
    private bool isGoingDown = false; // Bool for if the popup is going down


    // Update is called once per frame
    void Update()
    {
        if (m_popupOn && m_timer > 2.5f) // If the popup is on screen and has stayed there for 2.5 seconds
        {
            LeanTween.move(m_tBox, new Vector3(rectTransform.position.x, -200.0f, rectTransform.position.z), 1.0f); // Move transparent box down
            LeanTween.move(m_text, new Vector3(rectTransform.position.x, -200.0f, rectTransform.position.z), 1.0f); // Move text down

            m_popupOn = false; // Set the popup on bool to false
            m_timing = false; // Don't time anymore
            m_timer = 0.0f; // Reset timer

            isGoingDown = true; // Set this variable to true as the popup is now going down
        }

        if (m_timing) // If the timer should be timing
            m_timer += Time.deltaTime; // The timer is timing

        if (isGoingDown == true) // If the popup is going down
        {
            if (m_tBox.GetComponent<RectTransform>().position.y < -70.0f) // If it is off screen
            {
                m_tBox.SetActive(false);
                m_text.SetActive(false);
                isGoingDown = false; // The popup should no longer be going down
            }
        }
    }

    public void ShowQuestPopup()
    {
        if (m_tBox.transform.position.y < -70.0f && isGoingDown == false) // If the popup is not going down or is off screen
        {
            m_tBox.SetActive(true);
            m_text.SetActive(true);

            rectTransform = m_tBox.GetComponent<RectTransform>(); // Add rect transform
            RawImage image = m_tBox.GetComponent<RawImage>(); // Add image
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f); // Set box to have a transparent color

            // Set anchors
            rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            rectTransform.localScale = new Vector3(8.0f, 2.5f, 1.0f); // Set the rect transforms to a certain scale
            rectTransform.anchoredPosition = new Vector3(0.0f, -200.0f, 0.0f); // Set its position of screen

            TextMeshProUGUI text = m_text.GetComponent<TextMeshProUGUI>(); // Get text
            text.text = "Quest - '" + App.GetModule<QuestModule>().GetActiveQuest("Test").name + "' - has been added to your Quest Log"; // Set text
            text.alignment = TextAlignmentOptions.Center; // Set alignment to be the center

            RectTransform textTransform = m_text.GetComponent<RectTransform>(); // Get the text's rect transform

            // Set anchors
            textTransform.anchorMin = new Vector2(0.5f, 0.0f);
            textTransform.anchorMax = new Vector2(0.5f, 0.0f);
            textTransform.pivot = new Vector2(0.5f, 0.5f);

            textTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Set the text's scale
            textTransform.anchoredPosition = new Vector3(0.0f, -170.0f, 0.0f); // Set the text's position
            textTransform.sizeDelta = new Vector2(700, 250); // Set the text box's size

            LeanTween.move(m_tBox, new Vector3(rectTransform.position.x, 50.0f, rectTransform.position.z), 1.0f); // Make the transparent box move onto screen
            LeanTween.move(m_text, new Vector3(textTransform.position.x, 50.0f, textTransform.position.z), 1.0f); // Make the text move onto screen

            m_popupOn = true; // Set this bool to true 
            m_timing = true; // Make the timer time
        }
    }
}
