using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest")]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct StringBoolPair
    {
        public string key;

        public bool value;
    }

    [System.Serializable]
    public struct StringListIntPair
    {
        public string key;

        public List<int> value;

        public StringListIntPair(string in_key, List<int> in_int)
        {
            key = in_key;
            value = in_int;
        }
    }

    public new string name;

    [SerializeField]
    public List<StringBoolPair> Prerequisites = new List<StringBoolPair>();

    [SerializeField]
    public List<StringBoolPair> Requierments = new List<StringBoolPair>();

    public Dictionary<string, bool> DictRequierments = new Dictionary<string, bool>();
    public Dictionary<string, bool> DictPrerequisites = new Dictionary<string, bool>();
    public string activeDescription;
    public string completeDescription;
    public bool complete = false;
    public bool showMarker;
    public List<int> markerLocations;
    public string markerScene;

    private Quest()
    {
        foreach (StringBoolPair pair in Prerequisites)
        {
            DictRequierments.Add(pair.key, pair.value);
        }
        foreach (StringBoolPair pair in Requierments)
        {
            DictPrerequisites.Add(pair.key, pair.value);
        }
    }

    public Quest(StrippedQuest stripped)
    {
        name = stripped.name;
        Prerequisites = stripped.Prerequisites;
        Requierments = stripped.Requierments;
        activeDescription = stripped.activeDescription;
        completeDescription = stripped.completeDescription;
        complete = stripped.complete;
        showMarker = stripped.showMarker;
        markerLocations = stripped.markerLocations;
        markerScene = stripped.markerScene;

        foreach (StringBoolPair pair in Prerequisites)
        {
            DictRequierments.Add(pair.key, pair.value);
        }
        foreach (StringBoolPair pair in Requierments)
        {
            DictPrerequisites.Add(pair.key, pair.value);
        }
    }
}

[System.Serializable]
public class StrippedQuest
{
    public StrippedQuest(Quest quest)
    {
        name = quest.name;
        Prerequisites = quest.Prerequisites;
        Requierments = quest.Requierments;
        activeDescription = quest.activeDescription;
        completeDescription = quest.completeDescription;
        complete = quest.complete;
        showMarker = quest.showMarker;
        markerLocations = quest.markerLocations;
        markerScene = quest.markerScene;
    }

    public string name;

    public List<Quest.StringBoolPair> Prerequisites = new List<Quest.StringBoolPair>();

    public List<Quest.StringBoolPair> Requierments = new List<Quest.StringBoolPair>();

    public string activeDescription;
    public string completeDescription;
    public bool complete = false;
    public bool showMarker;
    public List<int> markerLocations;
    public string markerScene;
}