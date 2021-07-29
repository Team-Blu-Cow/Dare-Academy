using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using blu;

public class ShipParts : MonoBehaviour
{
    [SerializeField]
    private int maxParts;

    private void Start()
    {
        //GetComponentInChildren<TextMeshProUGUI>().text = "Ship Parts - " + App.GetModule<LevelModule>().ActiveSaveData.partsCollected + "/" + maxParts;
    }

    public void UpdateUI()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = "Ship Parts - " + App.GetModule<LevelModule>().ActiveSaveData.partsCollected + "/" + maxParts;
    }
}