using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFullObject : MonoBehaviour
{
    public GameObject mainObject;

    private void OnDestroy()
    {
        if(mainObject != null)
        {
            Destroy(mainObject);
        }
    }
}
