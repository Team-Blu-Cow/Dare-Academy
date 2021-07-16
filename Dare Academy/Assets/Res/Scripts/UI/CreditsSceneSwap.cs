using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CreditsSceneSwap : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        App.GetModule<SceneModule>().SwitchScene("MainMenu", TransitionType.LRSweep, LoadingBarType.BottomRightRadial);
    }
}