using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class AddQuest : MonoBehaviour, IInteractable
{
    private PlayerEntity m_player;
    private bool m_playerInRange = false;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            Debug.Log("Interacted");
            App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
        }
    }

    public bool PlayerInRange(Transform transform)
    {
        return false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        App.GetModule<InputModule>().PlayerController.Player.Interact.started += OnInteract;
        m_player = FindObjectOfType<PlayerEntity>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.m_interactToolTip.SetActive(true);
            m_playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.m_interactToolTip.SetActive(false);
            m_playerInRange = false;
        }
    }
}