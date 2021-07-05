using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;
using JUtil;

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
        position = grid.GetNodePosition(x, y);
        walkable = false;
        overridden = false;
        overrideType = NodeOverrideType.None;
        lvlTransitionInfo = null;
    }

    public GridNode()
    {
        position = new GridNodePosition();
        walkable = false;
        overridden = false;
        overrideType = NodeOverrideType.None;
        lvlTransitionInfo = null;
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

    public GridNode GetNeighbour(Vector2Int direction)
    {
        if (direction == null)
            return null;

        if (direction.x != 0 && direction.y != 0)
        {
            // diagonal moves are not cached by the grid, we will attempt to find a valid node manually

            Vector2Int dir_x = new Vector2Int(direction.x , 0);
            Vector2Int dir_y = new Vector2Int(0 , direction.y);

            // X -> Y
            {
                GridNode node_x = Neighbors[dir_x.RotationToIndex()].reference;
                if (node_x != null)
                {
                    GridNode node_x_y = node_x.Neighbors[dir_y.RotationToIndex()].reference;
                    {
                        if (node_x_y != null)
                        {
                            return node_x_y;
                        }
                    }
                }
            }

            // Y -> X
            {
                GridNode node_y = Neighbors[dir_y.RotationToIndex()].reference;
                if (node_y != null)
                {
                    GridNode node_y_x = node_y.Neighbors[dir_x.RotationToIndex()].reference;
                    {
                        if (node_y_x != null)
                        {
                            return node_y_x;
                        }
                    }
                }
            }
        }
        else
        {
            // resolve as normal
            return Neighbors[direction.RotationToIndex()].reference;
        }

        return null;
    }
}