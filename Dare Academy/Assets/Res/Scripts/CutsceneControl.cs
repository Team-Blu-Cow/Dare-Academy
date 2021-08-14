using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CutsceneControl : MonoBehaviour
{
    private float m_timer;

    private void OnDisable()
    {
        if (App.GetModule<SceneModule>())
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default));
        }
    }

    private void Start()
    {
        m_timer = 0.0f;
    }

    private void Update()
    {
        if(m_timer > 24.5f)
        {
            if (App.GetModule<SceneModule>())
            {
                App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default));
            }
        }

        m_timer += Time.deltaTime;
    }
}