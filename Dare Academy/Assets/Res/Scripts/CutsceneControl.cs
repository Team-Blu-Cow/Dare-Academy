using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CutsceneControl : MonoBehaviour
{
    private void OnDisable()
    {
        App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default));
    }
}