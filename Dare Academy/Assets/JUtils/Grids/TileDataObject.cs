using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace JUtil.Grids
{
    // TILE DATA SCRIPTABLE OBJECT ******************************************************************************************************************
    [CreateAssetMenu(menuName = "JUtils/TileDataObject")]
    public class TileDataObject : ScriptableObject
    {
        public TileBase[] tiles;

        public TileData data;

        public NeighbourGraph neighbours;

        public TileDataObject()
        {
            neighbours = new NeighbourGraph();
        }
    }

    // TILE DATA FAUX "DICTIONARY" ****************************************************************************************************************** 
    [System.Serializable]
    public class TileData
    {
        [SerializeField] public DataPair<bool>[]   boolData = new DataPair<bool>[1];
        [SerializeField] public DataPair<int>[]    intData = new DataPair<int>[1];
        [SerializeField] public DataPair<float>[]  floatData = new DataPair<float>[1];

        public bool GetDataBool(string name, out bool output)
        {
            foreach (var item in boolData)
            {
                if (item.name == name)
                {
                    output = item.data;
                    return true;
                }
            }

            output = false;
            return false;
        }

        public bool GetDataInt(string name, out int output)
        {
            foreach (var item in intData)
            {
                if (item.name == name)
                {
                    output = item.data;
                    return true;
                }
            }

            output = -0;
            return false;
        }

        public bool GetDataFloat(string name, out float output)
        {
            foreach (var item in intData)
            {
                if (item.name == name)
                {
                    output = item.data;
                    return true;
                }
            }

            output = -0;
            return false;
        }

    }

    // DATA PAIR STRUCT *****************************************************************************************************************************
    [System.Serializable]
    public struct DataPair<T>
    {
        public string name;
        public T data;
    }

    // NEIGHBOUR GRAPH STRUCT ***********************************************************************************************************************
    public enum STATE : int
    {
        OFF = 0,
        TWOWAY = 1,
        ONEWAY = 2,

        NumberOfStates
    }

    [System.Serializable]
    public class NeighbourGraph
    {
        [SerializeField] public STATE[] neighbors;

        public NeighbourGraph()
        {
            neighbors = new STATE[8] 
            {
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
                STATE.OFF,
            };
        }

        public STATE this[int i]
        {
            get { return neighbors[i]; }
            set { neighbors[i] = value; }
        }

        public STATE N
        {
            get { return neighbors[1]; }
            set { neighbors[1] = value; }
        }
        public STATE NE
        {
            get { return neighbors[2]; }
            set { neighbors[2] = value; }
        }
        public STATE E
        {
            get { return neighbors[4]; }
            set { neighbors[4] = value; }
        }
        public STATE SE
        {
            get { return neighbors[7]; }
            set { neighbors[7] = value; }
        }
        public STATE S
        {
            get { return neighbors[6]; }
            set { neighbors[6] = value; }
        }
        public STATE SW
        {
            get { return neighbors[5]; }
            set { neighbors[5] = value; }
        }
        public STATE W
        {
            get { return neighbors[4]; }
            set { neighbors[4] = value; }
        }
        public STATE NW
        {
            get { return neighbors[0]; }
            set { neighbors[0] = value; }
        }

    }


}