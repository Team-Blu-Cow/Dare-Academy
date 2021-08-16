using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class PlayerPickup : Interface
{
    private enum Type
    {
        Health,
        Energy,
        ShipPart,
        Helmet
    }

    [SerializeField] private Type m_type;
    [SerializeField] private GameEventFlags.Flags m_flagToFlip;

    [SerializeField] private PlayerUI m_playerUI;
    [SerializeField] private ShipParts m_shipParts;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags(m_flagToFlip, true);
            blu.App.GetModule<blu.LevelModule>().SaveGame();

            GameObject popup = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/PopUp"));

            PopUpController popUpController = popup.GetComponentInChildren<PopUpController>();

            popUpController.m_playerControlled = false;

            switch (m_type)
            {
                case Type.Health:

                    popUpController.m_head = "Health";
                    popUpController.m_body = new List<string>() { "Your health has now increased" };

                    m_player.MaxHealth++;
                    m_player.Health++;
                    m_playerUI.AddHealth();
                    break;

                case Type.Energy:

                    popUpController.m_head = "Energy";
                    popUpController.m_body = new List<string>() { "Your energy has now increased" };

                    m_player.MaxEnergy++;
                    m_player.Energy++;
                    m_playerUI.AddEnergy();
                    break;

                case Type.ShipPart:

                    popUpController.m_head = "ShipPart";
                    popUpController.m_body = new List<string>() { "You find a new part of your ship" };

                    m_player.ShipParts++;
                    App.GetModule<LevelModule>().ActiveSaveData.partsCollected++;

                    break;

                case Type.Helmet:

                    Destroy(popup);
                    App.GetModule<DialogueModule>().StartDialogue(transform.parent.gameObject);

                    break;

                default:
                    break;
            }

            Destroy(gameObject);
        }
    }

    public override void Start()
    {
        base.Start();

        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet(m_flagToFlip))
            Destroy(gameObject);
    }

    public override void OnValidate()
    {
        base.OnValidate();

        m_playerUI = FindObjectOfType<PlayerUI>();
        m_shipParts = FindObjectOfType<ShipParts>();
    }
}