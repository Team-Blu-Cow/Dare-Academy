using System;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void loadApplication()
    {
        GameObject application = Instantiate(Resources.Load<GameObject>("Application"));
        if (application == null)
        {
            throw new ApplicationException();
        }
        DontDestroyOnLoad(application);
    }
}