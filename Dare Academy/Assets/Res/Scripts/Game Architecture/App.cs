using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;

namespace blu
{
    public class App : MonoBehaviour
    {
        // Singleton instance
        [HideInInspector] private static App instance = null;

        private List<blu.Module> _modules = new List<blu.Module>();
        private blu.ModuleManager _moduleManager = new blu.ModuleManager();
        private CanvasTool.CanvasManager _canvasManager = null;
        private blu.CamContoller _cameraController = null;
        private blu.Jukebox _jukebox = null;

        [HideInInspector] public static List<blu.Module> LoadedModules { get => instance._modules; }
        [HideInInspector] public static Transform Transform { get => instance.transform; }
        [HideInInspector] public static CanvasTool.CanvasManager CanvasManager { get => instance._canvasManager; }
        [HideInInspector] public static blu.CamContoller CameraController { get => instance._cameraController; }
        [HideInInspector] public static blu.Jukebox Jukebox { get => instance._jukebox; }

        // set to true if different load orders are required due to
        // start up scenes.
        //private const bool _SceneDependantLoadOrder = false;

        private void Awake()
        {
            #region Singleton Code

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            FindNonPersistantManagers();
            SceneManager.sceneLoaded += FindNonPersistantManagers;

            #endregion Singleton Code

            #region Per-Scene Switch Block

            switch (SceneManager.GetActiveScene().name)
            {
                case "SplashScreen":

                    AddBaselineModules();
                    break;

                case "MainMenu":

                    AddBaselineModules();
                    break;

                default: // default load order

                    AddBaselineModules();
                    break;
            }

            #endregion Per-Scene Switch Block
        }

        private void AddBaselineModules()
        {
            _moduleManager.AddModule<blu.LevelModule>();
            _moduleManager.AddModule<blu.SettingsModule>();
            _moduleManager.AddModule<blu.SceneModule>();
            _moduleManager.AddModule<blu.InputModule>();
            _moduleManager.AddModule<blu.DialogueModule>();
            _moduleManager.AddModule<blu.QuestModule>();
            //_moduleManager.AddModule<blu.AudioModule>(); // not nessessary, included for readablility
        }

        private void FindNonPersistantManagers(Scene newScene = new Scene(), LoadSceneMode sceneMode = new LoadSceneMode())
        {
            FindCanvasManager();
            FindJukebox();
            FindCameraController();
            FindEventSystem();
        }

        private static void FindEventSystem()
        {
            if (App.GetModule<DialogueModule>() != null)
            {
                UnityEngine.EventSystems.EventSystem temp = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
                if (temp != null)
                {
                    App.GetModule<DialogueModule>().EventSystem = temp.gameObject;
                    Debug.Log("[App]: Event System found.");
                }
                else
                {
                    Debug.Log("[App]: Event System is null.");
                }
            }
        }

        private void FindCameraController()
        {
            _cameraController = null;
            _cameraController = FindObjectOfType<blu.CamContoller>();
            if (_cameraController)
                Debug.Log("[App]: Camera Controller found.");
            else
                Debug.Log("[App]: Camera Controller is null.");
        }

        private void FindJukebox()
        {
            _jukebox = null;
            _jukebox = FindObjectOfType<blu.Jukebox>();
            if (_jukebox)
                if (_canvasManager)
                    Debug.Log("[App]: Jukebox found.");
                else
                    Debug.Log("[App]: Jukebox is null.");
        }

        private void FindCanvasManager()
        {
            _canvasManager = null;
            _canvasManager = FindObjectOfType<CanvasTool.CanvasManager>();
            if (_canvasManager)
                Debug.Log("[App]: Canvas Manager found.");
            else
                Debug.Log("[App]: Canvas Manager is null.");
        }

        public static T GetModule<T>() where T : blu.Module
        {
            MethodInfo method = typeof(blu.ModuleManager).GetMethod(nameof(GetModule));
            method = method.MakeGenericMethod(typeof(T));
            return (T)method.Invoke(instance._moduleManager, null);
        }

        public static void AddModule<T>() where T : blu.Module
        {
            MethodInfo method = typeof(blu.ModuleManager).GetMethod(nameof(AddModule));
            method = method.MakeGenericMethod(typeof(T));
            method.Invoke(instance._moduleManager, null);
        }

        public static void RemoveModule<T>() where T : blu.Module
        {
            MethodInfo method = typeof(blu.ModuleManager).GetMethod(nameof(RemoveModule));
            method = method.MakeGenericMethod(typeof(T));
            method.Invoke(instance._moduleManager, null);
        }
    }
}