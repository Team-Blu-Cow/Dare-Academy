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
}