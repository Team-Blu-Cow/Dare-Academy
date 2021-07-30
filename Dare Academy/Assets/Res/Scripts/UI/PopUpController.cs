using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PopUpController : MonoBehaviour
{
    private PlayableDirector m_playableDirector;

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

    public void StartPopUp()
    {
        StopTimeline();

        //Do convo

        StartTimeline();
    }

    private void StartTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    private void StopTimeline()
    {
        m_playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
}