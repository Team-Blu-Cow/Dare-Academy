using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.InputSystem;

public class UpgradePlayer : Interface
{
    private enum UpgradeType
    {
        Shoot = 1,
        Dash = 2,
        Block = 4
    }

    [SerializeField] private UpgradeType upgradeType;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags((Int32)upgradeType, true);
            PlayerEntity.Instance.Abilities.Refresh();

            FindObjectOfType<PlayerUI>().UpdateUI();

            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    public override async void Start()
    {
        base.Start();

        await App.GetModule<LevelModule>().AwaitInitialised();

        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet((Int32)upgradeType))
            Destroy(gameObject);
    }
}