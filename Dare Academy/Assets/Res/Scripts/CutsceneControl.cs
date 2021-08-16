using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;
using TMPro;

public class CutsceneControl : MonoBehaviour
{
    private float m_timer = 0f;

    [SerializeField]
    private TextMeshProUGUI m_text;

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.UI.Pause.started += Skip;
        App.GetModule<InputModule>().PlayerController.Player.Interact.started += Skip;
        App.GetModule<InputModule>().SystemController.UI.Skip.started += Skip;
    }

    private void OnDisable()
    {
        if (App.GetModule<SceneModule>())
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default), TransitionType.Fade);
        }

        App.GetModule<InputModule>().SystemController.UI.Skip.started -= Skip;
        App.GetModule<InputModule>().SystemController.UI.Pause.started -= Skip;
    }

    private void Skip(InputAction.CallbackContext context)
    {
        if (App.GetModule<SceneModule>() && m_text.isActiveAndEnabled)
        {
            App.GetModule<SceneModule>().SwitchScene(LevelModule.ResolveSceneNameString(blu.LevelID._default), TransitionType.Fade);
        }

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

            m_text.text = "Press " + key + " to skip";
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