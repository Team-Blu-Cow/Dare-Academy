using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using blu;

public class ShipParts : MonoBehaviour
{
    [SerializeField]
    private int maxParts;

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.UI.Map.performed += UpdateUI;
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().SystemController.UI.Map.performed -= UpdateUI;
    }

    private void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = "Ship Parts - " + App.GetModule<LevelModule>().ActiveSaveData.partsCollected + "/" + maxParts;
    }

    public void UpdateUI(InputAction.CallbackContext context)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = "Ship Parts - " + App.GetModule<LevelModule>().ActiveSaveData.partsCollected + "/" + maxParts;
    }
}