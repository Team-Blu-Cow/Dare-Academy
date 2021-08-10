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
        transform.GetChild(1).GetComponent<Image>().sprite = KeyboardSprites[13];

        baseTransform.GetComponentsInChildren<Image>()[0].sprite = KeyboardSprites[10];     // Move up (W)
        baseTransform.GetComponentsInChildren<Image>()[1].sprite = KeyboardSprites[3];      // Move left (A)
        baseTransform.GetComponentsInChildren<Image>()[2].sprite = KeyboardSprites[9];      // Move down (S)
        baseTransform.GetComponentsInChildren<Image>()[3].sprite = KeyboardSprites[8];      // Move right (D)
        baseTransform.GetComponentsInChildren<Image>()[4].sprite = KeyboardSprites[16];     // Wait (Q)
        baseTransform.GetComponentsInChildren<Image>()[5].sprite = KeyboardSprites[17];     // Ability Mode (SPC)
        baseTransform.GetComponentsInChildren<Image>()[6].sprite = KeyboardSprites[0];      // Left Ability (1)
        baseTransform.GetComponentsInChildren<Image>()[7].sprite = KeyboardSprites[1];      // Right Ability (2)
        baseTransform.GetComponentsInChildren<Image>()[8].sprite = KeyboardSprites[12];     // Pause (ESC)
        baseTransform.GetComponentsInChildren<Image>()[9].sprite = KeyboardSprites[14];     // Map (M)
        baseTransform.GetComponentsInChildren<Image>()[10].sprite = KeyboardSprites[15];    // Zoom map In (SCR WHL)
        baseTransform.GetComponentsInChildren<Image>()[11].sprite = KeyboardSprites[15];    // Zoom map Out (SCR WHL)
        baseTransform.GetComponentsInChildren<Image>()[12].sprite = KeyboardSprites[11];    // Interact (E)
        baseTransform.GetComponentsInChildren<Image>()[13].sprite = KeyboardSprites[2];    // Cancel Ability (BCK SPC)
    }

    private void SetController()
    {
        Transform baseTransform = transform.GetChild(2);
        transform.GetChild(1).GetComponent<Image>().sprite = ControllerSprites[1];

        baseTransform.GetComponentsInChildren<Image>()[0].sprite = ControllerSprites[9];    // Move up (D-PAD Up)
        baseTransform.GetComponentsInChildren<Image>()[1].sprite = ControllerSprites[7];    // Move left (D-PAD left)
        baseTransform.GetComponentsInChildren<Image>()[2].sprite = ControllerSprites[6];    // Move down (D-PAD down)
        baseTransform.GetComponentsInChildren<Image>()[3].sprite = ControllerSprites[8];    // Move right (D-PAD right)
        baseTransform.GetComponentsInChildren<Image>()[4].sprite = ControllerSprites[15];   // Wait (Y)
        baseTransform.GetComponentsInChildren<Image>()[5].sprite = ControllerSprites[14];   // Ability Mode (X)
        baseTransform.GetComponentsInChildren<Image>()[6].sprite = ControllerSprites[2];    // Left Ability (RB)//
        baseTransform.GetComponentsInChildren<Image>()[7].sprite = ControllerSprites[3];    // Right Ability (LB)//
        baseTransform.GetComponentsInChildren<Image>()[8].sprite = ControllerSprites[11];   // Pause (Menu)
        baseTransform.GetComponentsInChildren<Image>()[9].sprite = ControllerSprites[13];   // Map (Select)
        baseTransform.GetComponentsInChildren<Image>()[10].sprite = ControllerSprites[10];   // Zoom map In (LT)
        baseTransform.GetComponentsInChildren<Image>()[11].sprite = ControllerSprites[12];  // Zoom map Out (RT)
        baseTransform.GetComponentsInChildren<Image>()[12].sprite = ControllerSprites[0];   // Interact (A)//
        baseTransform.GetComponentsInChildren<Image>()[13].sprite = ControllerSprites[4];  // Cancel Ability (B)
    }
}