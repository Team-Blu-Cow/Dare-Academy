using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class QuestModule : Module
{
    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();
    public List<Quest> ActiveQuests { get => _activeQuests; }
    public List<Quest> CompletedQuests { get => _completedQuests; }

    public bool AddQuest(Quest in_quest)
    {
        if (in_quest)
        {
            _activeQuests.Add(in_quest);
            return true;
        }
        return false;
    }

    public bool SetRequiermentsComplete(Quest in_quest, string in_req)
    {
        if (in_quest)
        {
            in_quest.DictPrerequisites[in_req] = true;
            return true;
        }
        return false;
    }

    public bool SetRequiermentsComplete(string in_name, string in_req) => SetRequiermentsComplete(_activeQuests.Find(x => x.name.Contains(in_name)), in_req);

    public bool SetComplete(Quest in_quest)
    {
        if (in_quest)
        {
            in_quest.complete = true;
            _completedQuests.Add(in_quest);
            _activeQuests.Remove(in_quest);
            return true;
        }
        return false;
    }

    public bool SetComplete(string in_name) => SetComplete(_activeQuests.Find(x => x.name.Contains(in_name)));

    public bool AbandonQuest(Quest in_quest)
    {
        Quest holder = _activeQuests.Find(x => x == in_quest);
        if (holder)
        {
            _activeQuests.Remove(in_quest);
            return true;
        }
        return false;
    }

    public bool AbandonQuest(string in_name) => AbandonQuest(_activeQuests.Find(x => x.name.Contains("in_name")));

    public Quest GetActiveQuest(string in_name)
    {
        Quest holder = _activeQuests.Find(x => x.name == in_name);
        if (holder)
        {
            return holder;
        }
        return null;
    }

    public Quest GetCompletedQuest(string in_name)
    {
        Quest holder = _completedQuests.Find(x => x.name == in_name);
        if (holder)
        {
            return holder;
        }
        return null;
    }
}