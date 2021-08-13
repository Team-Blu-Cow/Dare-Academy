using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;

public class MirInteract : Interface
{
    [System.Serializable]
    public enum State
    {
        NotInteracted,
        GivenQuest,
        CompletedQuest
    }

    [SerializeField] private GameObject m_notInteractedDialouge;
    [SerializeField] private GameObject m_givenQuestDialouge;
    [SerializeField] private GameObject m_thirdDialouge;
    [SerializeField] private GameObject m_nonCompleteDialogue;

    [SerializeField] private Quest m_quest;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        LevelModule levelModule = App.GetModule<LevelModule>();
        DialogueModule dialogueModule = App.GetModule<DialogueModule>();

        if (m_playerInRange)
        {
            switch (levelModule.ActiveSaveData.mirState)
            {
                case State.NotInteracted:

                    dialogueModule.StartDialogue(m_notInteractedDialouge);

                    StartCoroutine(WaitDialougeNotInteracted());

                    levelModule.ActiveSaveData.mirState = State.GivenQuest;
                    levelModule.SaveGame();

                    break;

                case State.GivenQuest:

                    dialogueModule.StartDialogue(m_givenQuestDialouge);

                    StartCoroutine(WaitDialougeQuest());

                    levelModule.ActiveSaveData.mirState = State.CompletedQuest;
                    levelModule.SaveGame();

                    break;

                case State.CompletedQuest:

                    if (App.GetModule<QuestModule>().GetCompletedQuest(m_quest.name) != null)
                        dialogueModule.StartDialogue(m_thirdDialouge);
                    else
                        dialogueModule.StartDialogue(m_nonCompleteDialogue);

                    levelModule.SaveGame();
                    break;

                default:
                    break;
            }
        }
    }

    private IEnumerator WaitDialougeNotInteracted()
    {
        while (App.GetModule<DialogueModule>().DialogueActive)
            yield return null;

        GameObject popup = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/PopUp"));

        PopUpController popUpController = popup.GetComponentInChildren<PopUpController>();

        popUpController.m_playerControlled = false;

        popUpController.m_head = "ShipPart";
        popUpController.m_body = new List<string>() { "You find a new part of your ship" };

        m_player.ShipParts++;
        App.GetModule<LevelModule>().ActiveSaveData.partsCollected++;
    }

    private IEnumerator WaitDialougeQuest()
    {
        while (App.GetModule<DialogueModule>().DialogueActive)
            yield return null;

        App.GetModule<QuestModule>().AddQuest(m_quest, true);
    }
}