using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptedActionQueue : ScriptableObject
{
    // values are set to prevent scripts being damaged when features are added or removed
    [System.Serializable]
    public enum ActionType // 7
    {
        None = 0,
        WaitTurns = 1,              // int
        Move = 2,                   // MoveData
        Dialogue = 3,               // prefab
        WaitPlayerEnterTrigger = 4, // gameObject
        WaitPlayerExitTrigger = 6,  // gameObject
        SetFlagValue =  7,          // int, bool
        Kill = 5,
    }

    [System.Serializable]
    public enum MoveDirections
    {
        North,
        East,
        South,
        West,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }

    [System.Serializable]
    public class MoveData
    {
        [SerializeField] public MoveDirections direction = MoveDirections.North;
        [SerializeField] public int distance = 1;
    }

    [System.Serializable]
    public class ActionWrapper
    {
        [SerializeField] public ActionType type = ActionType.None;

        [SerializeField] public int intData = 0;
        [SerializeField] public MoveData moveData = new MoveData();
        [SerializeField] public string textData = "";
        [SerializeField] public GameObject gameObject;
        [SerializeField] public bool boolData;
    }

    [SerializeField] public List<ActionWrapper> m_actionList = new List<ActionWrapper>();
}