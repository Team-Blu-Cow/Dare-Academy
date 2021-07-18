using System;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;

namespace blu
{
    public class ModuleManager
    {
        //  [Obsolete]
        //  public T GetModule<T>() where T : blu.Module
        //  {
        //      using (var timer = new Timer("GetModule"))
        //      {
        //          foreach (Module module in App.LoadedModules)
        //          {
        //              if (module.type == typeof(T))
        //              {
        //                  return (T)module;
        //              }
        //          }
        //          return null;
        //      }
        //  }

        public T GetModule<T>() where T : blu.Module
        {
            //using (var timer = new Timer("GetModule"))
                return (T)App.LoadedModules.Find(x => typeof(T) == x.type);
        }

        public void AddModule<T>() where T : blu.Module
        {
            //
            if (typeof(T) == typeof(Module))
            {
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: Attempted instantiation of abstract module type: " + typeof(T).ToString());
                return;
            }

            // Check for duplicate modules
            try
            {
                foreach (Module module in App.LoadedModules)
                {
                    if (module.type == typeof(T))
                    {
                        UnityEngine.Debug.Log("[App/ModuleManager]: Duplicate Module: " + typeof(T).ToString());
                        return;
                    }
                }
            }
            catch (System.Exception EXGenericBadType)
            {
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: Exception thrown during duplicate check");
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: Generic: " + typeof(T).ToString());
                UnityEngine.Debug.LogException(EXGenericBadType);
                return;
            }

            // prefab load try catch
            try
            {
                GameObject.Instantiate(Resources.Load<GameObject>(typeof(T).ToString())).transform.parent = App.Transform;
                App.LoadedModules.Add(App.Transform.GetComponentInChildren<T>());
            }
            catch (System.Exception EXcantFindPrefab)
            {
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: Could not load module prefab: \"" + typeof(T).ToString() + "\"");
                UnityEngine.Debug.LogException(EXcantFindPrefab);
                return;
            }

            // dependancy loop try catch
            try
            {
                // check module for dependancies
                foreach (Type dependancy in App.LoadedModules[App.LoadedModules.Count - 1].Dependancies)
                {
                    try // per dependancy try catch
                    {
                        MethodInfo method = typeof(ModuleManager).GetMethod(nameof(AddModule));
                        method = method.MakeGenericMethod(dependancy);
                        method.Invoke(this, null);
                    }
                    catch (System.Exception EXInvalidDependancy)
                    {
                        UnityEngine.Debug.LogWarning("[App/ModuleManager]: Could not load module \"" + dependancy.ToString() + "\"");
                        UnityEngine.Debug.LogWarning("[App/ModuleManager]: AddModule<T>() failed to load dependancy");
                        UnityEngine.Debug.LogException(EXInvalidDependancy);
                    }
                }
            }
            catch (System.Exception EXDependancyLoadFailure)
            {
                UnityEngine.Debug.LogWarning("[App]: Could not load dependancies ");
                UnityEngine.Debug.LogException(EXDependancyLoadFailure);
                return;
            }

            // initialization try catch
            try
            {
                GetModule<T>().Initialize();
            }
            catch (System.Exception EXFailedToInitModule)
            {
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: Could not initialize module \"" + typeof(T).ToString() + "\"");
                UnityEngine.Debug.LogWarning("[App/ModuleManager]: GetModule<T>().Initialize failed");
                UnityEngine.Debug.LogException(EXFailedToInitModule);
                return;
            }
        }

        public void RemoveModule<T>() where T : blu.Module
        {
            foreach (Module module in App.LoadedModules)
            {
                if (module.type == typeof(T))
                {
                    App.LoadedModules.Remove(module);
                    GameObject.Destroy(module.gameObject);
                    return;
                }
            }

            UnityEngine.Debug.Log("[App/ModuleManager]: Failed to find: " + typeof(T).ToString());
        }
    }
}