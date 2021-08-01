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
        Block = 4,
    }

    [SerializeField] private GameObject Dialouge;
    [SerializeField] private UpgradeType upgradeType;

    private bool m_showPopUp;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags((Int32)upgradeType, true);
            PlayerEntity.Instance.Abilities.Refresh();

            switch (upgradeType)
            {
                case UpgradeType.Shoot:
                    App.GetModule<DialogueModule>().StartDialogue(Dialouge);
                    PlayerEntity.Instance.Abilities.SetActiveAbility(PlayerAbilities.AbilityEnum.Shoot);

                    GameObject popup = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/PopUp"));

                    PopUpController popUpController = popup.GetComponentInChildren<PopUpController>();

                    popUpController.m_head = "Gun";
                    popUpController.m_body = new List<string>() { "Press a to fire", "Testing fire" };
                    popUpController.m_playerControlled = false;
                    break;

                case UpgradeType.Dash:
                    App.GetModule<DialogueModule>().StartDialogue(Dialouge);
                    break;

                case UpgradeType.Block:
                    App.GetModule<DialogueModule>().StartDialogue(Dialouge);
                    break;

                default:
                    break;
            }

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