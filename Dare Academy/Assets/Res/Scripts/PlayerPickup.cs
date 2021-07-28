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
        ShipPart
    }

    [SerializeField] private Type m_type;
    [SerializeField] private GameEventFlags.Flags m_flagToFlip;

    private PlayerUI m_playerUI;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags(m_flagToFlip, true);

            switch (m_type)
            {
                case Type.Health:

                    m_player.MaxHealth++;
                    m_player.Health++;
                    m_playerUI.AddHealth();
                    break;

                case Type.Energy:

                    m_player.MaxEnergy++;
                    m_player.Energy++;
                    m_playerUI.AddEnergy();
                    break;

                case Type.ShipPart:

                    m_player.ShipParts++;

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
    }
}