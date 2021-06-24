using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using blu;

// had to change this due to a conflict with JUtils
public class MainTestScript : MonoBehaviour
{
    private bool _paused;

    #region Level Loading

    public void LoadLevel(string in_scene)
    {
        if (Random.value < 0.5f)
        {
            App.GetModule<SceneModule>().SwitchScene(in_scene, TransitionType.LRSweep);
            App.GetModule<SceneModule>().SwitchScene(in_scene, TransitionType.LRSweep);
        }
        else
        {
            App.GetModule<SceneModule>().SwitchScene(in_scene, TransitionType.Fade);
        }
    }

    public void LoadLevel(InputAction.CallbackContext context)
    {
        if (Random.value < 0.5f)
        {
            App.GetModule<SceneModule>().SwitchScene("Level02", TransitionType.LRSweep, LoadingBarType.BottomRightRadial);
        }
        else
        {
            App.GetModule<SceneModule>().SwitchScene("Level02", TransitionType.Fade, LoadingBarType.BottomRightRadial);
        }
    }

    #endregion Level Loading

    #region File IO

    // example of disk read/write
    private async void ExampleLoadData()
    {
        blu.IOModule ioModule = App.GetModule<IOModule>();

        // if no save files exist create a new one
        if (ioModule.saveSlots.Count == 0)
        {
            // the string provided here will be ehats displayed to the user
            // filename use unix timestamp to guarantee they are unique
            // yes, this means if you make 2 new save games within a second it wont work, no i not fixing that
            await ioModule.CreateNewSave("example save name");
        }

        // load the save from disk
        // the data within settings.saveSlots[0] includes display name to be shown to the user
        Task<bool> taskLoadSave = ioModule.LoadSaveAsync(ioModule.saveSlots[0]);

        // we can do other stuff here while waiting for file load
        await taskLoadSave;

        // we can now access ioModule.settings.savedata which has been read from disk
        // you should ensure LoadSaveAsync returned true, if it retuens false an error has occured
        // when we are finished we need to call iomodule.Save() to write updates to this file to disk
    }

    #endregion File IO

    private void PauseGame(InputAction.CallbackContext context)
    {
        _paused = !_paused;

        if (_paused)
        {
            Time.timeScale = 0f;
            App.CanvasManager.AddCanvas(Instantiate(Resources.Load<GameObject>("prefabs/PauseCanvas")));
        }
        else
        {
            Time.timeScale = 1f;
            App.CanvasManager.RemoveCanvasContainer("PauseCanvas(Clone)");
        }
    }

    private void OnDestroy()
    {
        App.GetModule<blu.InputModule>().PlayerController.Move.Direction.started -= LoadLevel;
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed -= PauseGame;
    }

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<blu.InputModule>().PlayerController.Move.Direction.started += LoadLevel;
        //App.ModuleManager.GetModule<blu.InputModule>().SystemController.UI.Pause.performed += InputDebug;
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed += PauseGame;
        ExampleLoadData();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            App.RemoveModule<SettingsModule>();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            App.AddModule<SettingsModule>();
        }
    }
}