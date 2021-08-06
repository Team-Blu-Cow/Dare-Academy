using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;
using System;

public class Controls : MonoBehaviour
{
    private Sprite[] ControllerSprites;
    private Sprite[] KeyboardSprites;

    // Start is called before the first frame update
    private void Start()
    {
        ControllerSprites = Resources.LoadAll<Sprite>("GFX/ButtonImages/Controller");
        KeyboardSprites = Resources.LoadAll<Sprite>("GFX/ButtonImages/Keyboard");

        App.GetModule<InputModule>().LastDeviceChanged += DeviceChanged;
    }

    private void OnDestroy()
    {
        App.GetModule<InputModule>().LastDeviceChanged -= DeviceChanged;
    }

    public void ForceUpdate()
    {
        SwitchDevice();
    }

    private void DeviceChanged()
    {
        var controlsCanvas = App.CanvasManager.GetCanvasContainer("Controls");
        if (controlsCanvas != null && controlsCanvas.canvas.isActiveAndEnabled)
        {
            SwitchDevice();
        }
    }

    private void SwitchDevice()
    {
        switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
        {
            case "Keyboard":
                SetKeyboard();
                break;

            case "Mouse":
                SetKeyboard();
                break;

            case "Xbox Controller":
                SetController();
                break;

            case "Wireless Controller":
                SetController();
                break;
        }
    }

    private void SetKeyboard()
    {
        Transform baseTransform = transform.GetChild(2);
        transform.GetChild(1).GetComponent<Image>().sprite = KeyboardSprites[12];

        baseTransform.GetComponentsInChildren<Image>()[0].sprite = KeyboardSprites[9];
        baseTransform.GetComponentsInChildren<Image>()[1].sprite = KeyboardSprites[2];
        baseTransform.GetComponentsInChildren<Image>()[2].sprite = KeyboardSprites[8];
        baseTransform.GetComponentsInChildren<Image>()[3].sprite = KeyboardSprites[7];
        baseTransform.GetComponentsInChildren<Image>()[4].sprite = KeyboardSprites[15];
        baseTransform.GetComponentsInChildren<Image>()[5].sprite = KeyboardSprites[16];
        baseTransform.GetComponentsInChildren<Image>()[6].sprite = KeyboardSprites[0];
        baseTransform.GetComponentsInChildren<Image>()[7].sprite = KeyboardSprites[1];
        baseTransform.GetComponentsInChildren<Image>()[8].sprite = KeyboardSprites[11];
        baseTransform.GetComponentsInChildren<Image>()[9].sprite = KeyboardSprites[13];
        baseTransform.GetComponentsInChildren<Image>()[10].sprite = KeyboardSprites[14];
        baseTransform.GetComponentsInChildren<Image>()[11].sprite = KeyboardSprites[14];
        baseTransform.GetComponentsInChildren<Image>()[12].sprite = KeyboardSprites[10];
    }

    private void SetController()
    {
        Transform baseTransform = transform.GetChild(2);
        transform.GetChild(1).GetComponent<Image>().sprite = ControllerSprites[1];

        baseTransform.GetComponentsInChildren<Image>()[0].sprite = ControllerSprites[8];
        baseTransform.GetComponentsInChildren<Image>()[1].sprite = ControllerSprites[6];
        baseTransform.GetComponentsInChildren<Image>()[2].sprite = ControllerSprites[5];
        baseTransform.GetComponentsInChildren<Image>()[3].sprite = ControllerSprites[7];
        baseTransform.GetComponentsInChildren<Image>()[4].sprite = ControllerSprites[14];
        baseTransform.GetComponentsInChildren<Image>()[5].sprite = ControllerSprites[13];
        baseTransform.GetComponentsInChildren<Image>()[6].sprite = ControllerSprites[2];
        baseTransform.GetComponentsInChildren<Image>()[7].sprite = ControllerSprites[3];
        baseTransform.GetComponentsInChildren<Image>()[8].sprite = ControllerSprites[10];
        baseTransform.GetComponentsInChildren<Image>()[9].sprite = ControllerSprites[12];
        baseTransform.GetComponentsInChildren<Image>()[10].sprite = ControllerSprites[9];
        baseTransform.GetComponentsInChildren<Image>()[11].sprite = ControllerSprites[11];
        baseTransform.GetComponentsInChildren<Image>()[12].sprite = ControllerSprites[0];
    }
}