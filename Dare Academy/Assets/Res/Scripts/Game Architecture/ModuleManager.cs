using System;
using System.Reflection;
using UnityEngine;

namespace blu
{
    public class ModuleManager
    {
        public T GetModule<T>() where T : blu.Module
        {
            foreach (Module module in App.LoadedModules)
            {
                if (module.type == typeof(T))
                {
                    return (T)module;
                }
            }
            return null;
        }

        public void AddModule<T>() where T : blu.Module
        {
            //
            if (typeof(T) == typeof(Module))
            {

                Debug.LogWarning("[App/ModuleManager]: Attempted instantiation of abstract module type: " + typeof(T).ToString());
                return;
            }

            // Check for duplicate modules
            try
            {
                foreach (Module module in App.LoadedModules)
                {
                    if (module.type == typeof(T))
                    {
                        Debug.Log("[App/ModuleManager]: Duplicate Module: " + typeof(T).ToString());
                        return;
                    }
                }
            }
            catch (System.Exception EXGenericBadType)
            {
                Debug.LogWarning("[App/ModuleManager]: Exception thrown during duplicate check");
                Debug.LogWarning("[App/ModuleManager]: Generic: " + typeof(T).ToString());
                Debug.LogException(EXGenericBadType);
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
                Debug.LogWarning("[App/ModuleManager]: Could not load module prefab: \"" + typeof(T).ToString() + "\"");
                Debug.LogException(EXcantFindPrefab);
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
                        Debug.LogWarning("[App/ModuleManager]: Could not load module \"" + dependancy.ToString() + "\"");
                        Debug.LogWarning("[App/ModuleManager]: AddModule<T>() failed to load dependancy");
                        Debug.LogException(EXInvalidDependancy);
                    }
                }
            }
            catch (System.Exception EXDependancyLoadFailure)
            {
                Debug.LogWarning("[App]: Could not load dependancies ");
                Debug.LogException(EXDependancyLoadFailure);
                return;
            }

            // initialization try catch
            try
            {
                GetModule<T>().Initialize();
            }
            catch (System.Exception EXFailedToInitModule)
            {
                Debug.LogWarning("[App/ModuleManager]: Could not initialize module \"" + typeof(T).ToString() + "\"");
                Debug.LogWarning("[App/ModuleManager]: GetModule<T>().Initialize failed");
                Debug.LogException(EXFailedToInitModule);
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

            Debug.Log("[App/ModuleManager]: Failed to find: " + typeof(T).ToString());
        }
    }
}