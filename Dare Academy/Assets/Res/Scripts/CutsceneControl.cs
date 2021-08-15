using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;
using TMPro;

public class CutsceneControl : MonoBehaviour
{
    private float m_timer = 0f;
    private TextMeshProUGUI m_text;

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.UI.Pause.performed += Skip;
        App.GetModule<InputModule>().SystemController.UI.Skip.performed += Skip;
    }

    private void OnDisable()
    {
        if (App.GetModule<SceneModule>())
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default), TransitionType.Fade);
        }

        App.GetModule<InputModule>().SystemController.UI.Skip.performed += Skip;
        App.GetModule<InputModule>().SystemController.UI.Pause.performed -= Skip;
    }

    private void Skip(InputAction.CallbackContext context)
    {
        if (!m_text.isActiveAndEnabled)
        {
            m_text.enabled = true;

            string key = "";

            switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
            {
                case "Keyboard":
                    key = "Enter";
                    break;

                case "Mouse":
                    key = "Enter";
                    break;

                case "Xbox Controller":
                    key = "'A'";
                    break;

                case "Wireless Controller":
                    key = "'X'";
                    break;

                default:

                    break;
            }

            m_text.text = "Press " + key + "to skip";
        }

        if (App.GetModule<SceneModule>() && m_text.isActiveAndEnabled)
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