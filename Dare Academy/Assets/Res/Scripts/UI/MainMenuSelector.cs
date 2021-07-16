using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using blu;

public class MainMenuSelector : MonoBehaviour
{
    private EventSystem ES;

    // Start is called before the first frame update
    private void Start()
    {
        ES = EventSystem.current;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}