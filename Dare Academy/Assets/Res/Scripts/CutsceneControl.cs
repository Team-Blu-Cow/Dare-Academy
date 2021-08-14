using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CutsceneControl : MonoBehaviour
{
    private void OnDisable()
    {
        if (App.GetModule<SceneModule>())
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default));
        }
    }
}