using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptedActionQueue : ScriptableObject
{
    // TODO @matthew
    // pathfinding
    // wait player in range
    // wait player out of range

    [System.Serializable]
    public enum ActionType
    {
        None,
        WaitTurns, // int
        Move, // MoveData
        WaitPlayerIsDistance, // int
        Dialogue, // prefab
        Kill,
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
    }

    [SerializeField] public List<ActionWrapper> m_actionList = new List<ActionWrapper>();
}