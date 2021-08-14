using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CutsceneControl : MonoBehaviour
{
    private float m_timer = 0f;

    private void OnDisable()
    {
        if (App.GetModule<SceneModule>())
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default), TransitionType.Fade);
        }
    }

    private void Update()
    {
        if (m_timer > 24.5f)
        {
            if (App.GetModule<SceneModule>())
            {
                Debug.LogWarning("[CutsceneControl] cutscene took to long, forcing scene switch");
                App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default), TransitionType.Fade);
            }
        }

        m_timer += Time.deltaTime;
    }
}