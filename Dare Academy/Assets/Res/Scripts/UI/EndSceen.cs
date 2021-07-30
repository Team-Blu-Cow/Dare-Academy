using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using blu;

public class EndSceen : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();

        LeanTween.value(text.gameObject, a => text.color = a, new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 2f).setOnComplete(
            () => { LeanTween.delayedCall(5, () => { App.GetModule<SceneModule>().SwitchScene("Credits"); }); });
    }
}