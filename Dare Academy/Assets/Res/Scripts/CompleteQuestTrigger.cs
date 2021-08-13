using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class CompleteQuestTrigger : MonoBehaviour
{
    [SerializeField] private Quest quest;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        App.GetModule<QuestModule>().SetComplete(quest);
    }
}