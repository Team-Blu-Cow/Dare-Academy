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
            App.GetModule<LevelModule>().SaveGame();
            PlayerEntity.Instance.Abilities.Refresh();

            switch (upgradeType)
            {
                case UpgradeType.Shoot:
                    App.GetModule<DialogueModule>().StartDialogue(Dialouge);
                    PlayerEntity.Instance.Abilities.SetActiveAbility(PlayerAbilities.AbilityEnum.Shoot);

                    GameObject popup = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/PopUp"));

                    PopUpController popUpController = popup.GetComponentInChildren<PopUpController>();

                    popUpController.m_head = "Gun";
                    popUpController.m_playerControlled = true;

                    string key = "";

                    switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
                    {
                        case "Keyboard":
                            key = "'Space'";
                            break;

                        case "Mouse":
                            key = "'Space'";
                            break;

                        case "Xbox Controller":
                            key = "'X'";
                            break;

                        case "Wireless Controller":
                            key = "'Square'";
                            break;
                    }

                    if (PlayerEntity.Instance.abilityInputMode == PlayerEntity.AbilityInputMode.Hold)
                    {
                        popUpController.m_body = new List<string>() { "Hold " + key + " to enter ability mode", "When in ability mode use the direction controls to use the ability", "The next step the ability will be used" };
                    }
                    else if (PlayerEntity.Instance.abilityInputMode == PlayerEntity.AbilityInputMode.Toggle)
                    {
                        popUpController.m_body = new List<string>() { "Press " + key + " to toggle ability mode ", "When in ability mode use the direction controls to use the ability", "The next step the ability will be used" };
                    }
                    else
                    {
                        Debug.LogWarning("[UpgradePlayer] invalid AbilityInputMode set, grab @Jack or @Matthew to fix this");
                    }

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