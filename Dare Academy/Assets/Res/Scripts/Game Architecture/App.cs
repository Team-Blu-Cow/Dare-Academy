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

        [HideInInspector] public static List<blu.Module> LoadedModules { get => instance._modules; }
        [HideInInspector] public static Transform Transform { get => instance.transform; }
        [HideInInspector] public static CanvasTool.CanvasManager CanvasManager { get => instance._canvasManager; }

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
            FindSceneCanvasManager();
            SceneManager.sceneLoaded += FindSceneCanvasManager;

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
            _moduleManager.AddModule<blu.SettingsModule>();
            _moduleManager.AddModule<blu.SceneModule>();
            _moduleManager.AddModule<blu.InputModule>();
            _moduleManager.AddModule<blu.IOModule>();
            _moduleManager.AddModule<blu.DialogueModule>();
            //_moduleManager.AddModule<blu.AudioModule>(); // not nessessary, included for readablility
        }

        private void FindSceneCanvasManager(Scene newScene = new Scene(), LoadSceneMode sceneMode = new LoadSceneMode())
        {
            _canvasManager = null;
            _canvasManager = FindObjectOfType<CanvasTool.CanvasManager>();
            if (_canvasManager)
                Debug.Log("[App]: Canvas Manager found.");
            else
                Debug.Log("[App]: Canvas Manager is null.");

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