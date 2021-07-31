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

    [SerializeField] private string m_head;
    [SerializeField] private List<string> m_body;

    private bool m_nextDialogue = false;

    private void OnValidate()
    {
        m_playableDirector = GetComponentInParent<PlayableDirector>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnEnable()
    {
        Transform textTransform = transform.parent.GetChild(0);

        textTransform.GetComponentsInChildren<TextMeshProUGUI>()[0].text = m_head;
        textTransform.GetComponentsInChildren<TextMeshProUGUI>()[1].text = m_body[0];

        App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed += SkipDialogue;
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().DialogueController.Dialogue.Skip.performed -= SkipDialogue;
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
            yield return StartCoroutine(WaitForKeyDown());

            transform.parent.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[1].text = m_body[i];

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent.GetChild(0));

            i++;
        }

        yield return StartCoroutine(WaitForKeyDown());
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
        m_nextDialogue = true;
    }

    private void StartTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        App.GetModule<InputModule>().DialogueController.Dialogue.Disable();
    }

    private void StopTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        App.GetModule<InputModule>().DialogueController.Dialogue.Enable();
    }
}