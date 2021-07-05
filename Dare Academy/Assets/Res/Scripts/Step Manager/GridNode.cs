using System.Collections.Generic;
using JUtil.Grids;



[System.Serializable]
public class GridNode : IPathFindingNode<GridNode>, IHeapItem<GridNode>, MultiNode
{
    // INTERFACES *****************************************************************************
    public int gCost { get; set; } = 0;
    public int hCost { get; set; } = 0;
    public int fCost { get { return gCost + hCost; } }

    public int heapIndex { get; set; } = 0;

    public GridNodePosition position { get; set; }

    private NodeNeighborhood<GridNode> neighbors;
    public NodeNeighborhood<GridNode> Neighbors { get { return neighbors; } set { neighbors = value; } }
    public GridNode parent { get; set; }

    public int CompareTo(GridNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(other.hCost);

        return -compare;
    }

    public bool IsTraversable()
    {
        return walkable;
    }

    public int roomIndex { get; set; }
    public bool walkable { get; set; }
    public bool overridden { get; set; }
    public int overriddenDir { get; set; }
    public NodeOverrideType overrideType { get; set; }
    public LevelTransitionInformation lvlTransitionInfo { get; set; }

    // MEMBERS ********************************************************************************

    protected List<GridEntity> m_currentEntities = new List<GridEntity>();

    

    // METHODS ********************************************************************************
    public GridNode(Grid<GridNode> grid, int x, int y)
    {
        position            = grid.GetNodePosition(x, y);
        walkable            = false;
        overridden          = false;
        overrideType        = NodeOverrideType.None;
        lvlTransitionInfo   = null;
    }

    public GridNode()
    {
        position            = new GridNodePosition();
        walkable            = false;
        overridden          = false;
        overrideType        = NodeOverrideType.None;
        lvlTransitionInfo   = null;
    }

    public void AddEntity(GridEntity entity)
    {
        if (m_currentEntities.Contains(entity))
            return;
        m_currentEntities.Add(entity);
    }

    // returns false if object was not in the list
    public bool RemoveEntity(GridEntity entity)
    {
        return m_currentEntities.Remove(entity);
    }

    public bool CheckForConflict()
    {
        return (m_currentEntities.Count > 1);
    }

    public List<GridEntity> GetGridEntities()
    {
        return m_currentEntities;
    }
}