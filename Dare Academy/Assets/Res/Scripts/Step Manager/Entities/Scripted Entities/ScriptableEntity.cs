using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private ActionQueue m_actionQueue;

    [System.Serializable]
    public class ActionQueue : ScriptableObject
    {
        public enum ActionType
        {
            None,
            Wait,
            Move,
        }

        //public class ActionWrapper
        //{
        //    public ActionType type = ActionType.None;
        //    public object data = null;
        //}

        public List<Tuple<ActionType, object>> m_actionList;
    }
}