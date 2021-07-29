using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptedActionQueue : ScriptableObject
{
    // values are set to prevent scripts being damaged when features are added or removed
    [System.Serializable]
    public enum ActionType // 14
    {
        None = 0,
        WaitTurns = 1,              // int
        Move = 2,                   // MoveData
        Dialogue = 3,               // prefab
        AwaitDialogueComplete = 11, // void
        WaitPlayerEnterTrigger = 4, // gameObject
        WaitPlayerExitTrigger = 6,  // gameObject
        SetFlagEntityValue =  7,    // int, bool
        SetEventFlagValue = 8,      // int, bool
        SetCameraPosition = 9,      // bool , (false)vec3 OR (true)gameobject
        SetCameraToPlayer = 10,     // void
        ExecuteSteps = 12,          // int
        KillIfEventFlagSet = 13,    // int, bool (false)any OR (true)all
        KillIfEventFlagNotSet = 14, // int, bool (false)any OR (true)all
        Kill = 5,                   // void
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

        [SerializeField] public int int32Data = 0;
        [SerializeField] public double doubleData = 0d;
        [SerializeField] public MoveData moveData = new MoveData();
        [SerializeField] public string textData = "";
        [SerializeField] public GameObject gameObject;
        [SerializeField] public bool boolData;
        [SerializeField] public Vector3 vec3data;
    }

    [SerializeField] public List<ActionWrapper> m_actionList = new List<ActionWrapper>();
}