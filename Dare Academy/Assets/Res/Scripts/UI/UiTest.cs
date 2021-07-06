using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class UiTest : MonoBehaviour
{
    private bool _paused;

    private void OnDestroy()
    {
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed -= PauseGame;
    }

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed += PauseGame;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        _paused = !_paused;

        if (_paused)
        {
            App.CanvasManager.OpenCanvas("Options Menu", true);
            Time.timeScale = 0f;
        }
        else
        {
            App.CanvasManager.CloseCanvas();
            if (App.CanvasManager.topCanvas == null)
            {
                Time.timeScale = 1f;
            }
        }
    }
}