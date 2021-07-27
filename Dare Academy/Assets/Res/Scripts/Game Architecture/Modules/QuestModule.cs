using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu
{
    public class QuestModule : Module
    {
        private List<Quest> _activeQuests = new List<Quest>();
        private List<Quest> _completedQuests = new List<Quest>();
        public List<Quest> ActiveQuests { get => _activeQuests; }
        public List<Quest> CompletedQuests { get => _completedQuests; }

        public bool AddQuest(Quest in_quest)
        {
            if (in_quest == null)
                return false;

            foreach (var quest in _activeQuests)
            {                
                if (quest.name == in_quest.name)
                    return false;
            }



            foreach (var quest in _completedQuests)
            {
                if (quest.name == in_quest.name)
                    return false;
            }

			GameObject popUp = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/QuestPopUp"));
			popUp.GetComponentInChildren<QuestPopUp>().SetTitle(in_quest.name);
			popUp.GetComponentInChildren<QuestPopUp>().SetBody(in_quest.activeDescription);
			_activeQuests.Add(in_quest);
			return true;

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
            for (int i = _activeQuests.Count - 1; i >= 0; i--)
            {
                if (_activeQuests[i] == in_quest)
                {
                    _activeQuests.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool AbandonQuest(string in_name)
        {
            for (int i = _activeQuests.Count - 1; i >= 0; i--)
            {
                if (_activeQuests[i].name == in_name)
                {
                    _activeQuests.RemoveAt(i);
                    return true;
                }
            }
            return true;
        }

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

        public bool LoadFromFile()
        {
            LevelModule levelModule = App.GetModule<LevelModule>();

            List<StrippedQuest> active = levelModule.ActiveSaveData.activeQuests;
            List<StrippedQuest> completed = levelModule.ActiveSaveData.completedQuests;

            if (active != null)
            {
                for (int i = 0; i < active.Count; i++)
                {
                    if (active[i] != null)
                        AddQuest(new Quest(active[i]));
                }
            }

            if (completed != null)
            {
                for (int i = 0; i < completed.Count; i++)
                {
                    if (completed[i] != null)
                        SetComplete(new Quest(completed[i]));
                }
            }

            return true;
        }

        public bool WriteToFile()
        {
            LevelModule levelModule = App.GetModule<LevelModule>();

            levelModule.ActiveSaveData.activeQuests.Clear();
            levelModule.ActiveSaveData.completedQuests.Clear();

            for (int i = 0; i < _activeQuests.Count; i++)
            {
                levelModule.ActiveSaveData.activeQuests.Add(new StrippedQuest(_activeQuests[i]));
            }

            for (int i = 0; i < _completedQuests.Count; i++)
            {
                levelModule.ActiveSaveData.completedQuests.Add(new StrippedQuest(_completedQuests[i]));
            }

            return true;
        }

        protected override void SetDependancies()
        {
            _dependancies.Add(typeof(LevelModule));
        }
    }
}