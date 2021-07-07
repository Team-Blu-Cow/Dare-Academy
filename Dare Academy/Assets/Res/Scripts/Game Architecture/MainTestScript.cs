using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

using blu;

// had to change this due to a conflict with JUtils
public class MainTestScript : MonoBehaviour
{
    public class TempClass
    {
        public int testInteger = 0;
        public bool testBoolean = true;
        public float testFloat = 1f;
    }

    public TempClass test = new TempClass();

    private bool _paused;
    private bool _flipFlop = true;

    #region Level Loading

    public void LoadLevel(string in_scene)
    {
        if (UnityEngine.Random.value < 0.5f)
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
        if (UnityEngine.Random.value < 0.5f)
        {
            App.GetModule<SceneModule>().SwitchScene("Level02", TransitionType.LRSweep, LoadingBarType.BottomRightRadial);
        }
        else
        {
            App.GetModule<SceneModule>().SwitchScene("Level02", TransitionType.Fade, LoadingBarType.BottomRightRadial);
        }
    }

    #endregion Level Loading

    private void PauseGame(InputAction.CallbackContext context)
    {
        Debug.Log("Attempting Pause");

        _paused = !_paused;

        if (_paused)
        {
            Time.timeScale = 0f;
            App.CanvasManager.AddCanvas(Instantiate(Resources.Load<GameObject>("prefabs/PauseCanvas")));
            App.CanvasManager.OpenCanvas(App.CanvasManager.GetCanvasContainer("PauseCanvas(Clone)"), true);
        }
        else
        {
            Time.timeScale = 1f;
            App.CanvasManager.RemoveCanvasContainer("PauseCanvas(Clone)");
        }
    }

    private void OnDestroy()
    {
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed -= PauseGame;
    }

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<blu.InputModule>().SystemController.UI.Pause.performed += PauseGame;
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            App.GetModule<DialogueModule>().StartDialogue(Resources.Load<GameObject>("Conversations/ExampleZoneName/TestConvoTwo"));
        }

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            if (_flipFlop)
                App.CameraController.MoveToPosition(new Vector3(-10, 10, 0));
            else
                App.CameraController.MoveToCurrentRoom();

            _flipFlop = !_flipFlop;
        }
    }
}