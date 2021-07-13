using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float FramesPerSecond = 0f;
    [SerializeField] private TMPro.TMP_Text text;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("UpdateFPSCounter", 0, 1);
    }

    // Update is called once per frame
    private void UpdateFPSCounter()
    {
        FramesPerSecond = (int)(1f / Time.unscaledDeltaTime);
        text.text = "FPS: " + FramesPerSecond;
    }
}