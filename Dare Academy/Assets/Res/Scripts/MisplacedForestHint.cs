using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisplacedForestHint : MonoBehaviour
{
    public static MisplacedForestHint instance;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
}