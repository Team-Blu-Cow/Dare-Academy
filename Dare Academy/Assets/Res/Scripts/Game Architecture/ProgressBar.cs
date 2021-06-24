using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Image _progressBarImage;

    // Start is called before the first frame update

    private void Awake()
    {
        _progressBarImage = GetComponent<Image>();
        _progressBarImage.fillAmount = 0f;
    }

    public void UpdateProgressBar(float in_value)
    {
        _progressBarImage.fillAmount = in_value;
    }
}