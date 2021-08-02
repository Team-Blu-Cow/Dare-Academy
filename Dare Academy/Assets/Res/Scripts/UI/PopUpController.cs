using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using TMPro;
using blu;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
    private PlayableDirector m_playableDirector;

    public string m_head;
    public List<string> m_body;

    public bool m_playerControlled = false;

    private bool m_nextDialogue = false;
    private bool m_started = false;

    private void OnEnable()
    {
        m_playableDirector = GetComponentInParent<PlayableDirector>();

        App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed += SkipDialogue;

        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    private void Start()
    {
        Transform textTransform = transform.parent.GetChild(1);
        textTransform.GetComponentsInChildren<TextMeshProUGUI>()[0].text = m_head;
        textTransform.GetComponentsInChildren<TextMeshProUGUI>()[1].text = m_body[0];
    }

    private void Update()
    {
        if (!App.GetModule<DialogueModule>().DialogueActive && !m_started)
        {
            m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            m_started = true;

            if (m_playerControlled)
            {
                GetComponentInChildren<Image>().enabled = true; //show black screen
                StartControls();
            }
            else
            {
                GetComponentInChildren<Image>().enabled = false; //hide black screen
            }
        }
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed -= SkipDialogue;

        Destroy(transform.parent.gameObject);
    }

    public void StartPopUp()
    {
        StopTimeline();

        StartCoroutine(ShowPopup());
    }

    private IEnumerator ShowPopup()
    {
        int i = 1;
        while (i < m_body.Count)
        {
            if (m_playerControlled)
                yield return StartCoroutine(WaitForKeyDown());
            else
                yield return new WaitForSeconds(2);

            transform.parent.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>()[1].text = m_body[i];

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent.GetChild(1));

            i++;
        }

        if (m_playerControlled)
            yield return StartCoroutine(WaitForKeyDown());
        else
            yield return new WaitForSeconds(2);

        StartTimeline();
    }

    private IEnumerator WaitForKeyDown()
    {
        while (!m_nextDialogue)
            yield return null;

        m_nextDialogue = false;
    }

    private void SkipDialogue(InputAction.CallbackContext ctx)
    {
        if (!App.GetModule<DialogueModule>().DialogueActive)
            m_nextDialogue = true;
    }

    private void StartTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);

        App.GetModule<InputModule>().SystemController.UI.Enable();

        if (m_playerControlled)
            StopControls();
    }

    private void StopTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        App.GetModule<InputModule>().SystemController.UI.Disable();
    }

    private void StopControls()
    {
        App.GetModule<InputModule>().DialogueController.Dialogue.Disable();
        App.GetModule<InputModule>().PlayerController.Player.Enable();
    }

    private void StartControls()
    {
        App.GetModule<InputModule>().DialogueController.Dialogue.Enable();
        App.GetModule<InputModule>().PlayerController.Player.Disable();
    }
}