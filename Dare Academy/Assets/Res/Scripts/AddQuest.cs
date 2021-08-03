using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using blu;

public class AddQuest : Interface
{
    [SerializeField] private ShowQuestUI m_questPopup;

    [SerializeField] private Quest m_quest;

    public override void OnInteract(InputAction.CallbackContext ctx)
    {
        if (m_playerInRange)
        {
            if (m_quest)
            {
                App.GetModule<QuestModule>().AddQuest(m_quest, true);
                //m_questPopup.ShowQuestPopup();
            }
            else
            {
                Debug.LogWarning("[AddQuest.cs] m_quest was null");
            }
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();
        m_questPopup = GameObject.FindObjectOfType<ShowQuestUI>();
    }
}