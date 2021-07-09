using JUtil;
using JUtil.Grids;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// PATHFINDING WITH MULTIPLE GRIDS CLASS ************************************************************************************************************
[System.Serializable]
public class PathfindingMultiGrid
{
    // MEMBERS ************************************************************************************
    [SerializeField] private List<Grid<GridNode>> grids;

    [SerializeField] public GridInfo[] gridInfo;

    [SerializeField] private string[] gridNames;

    [SerializeField] private NodeOverrides<GridNode> nodeOverrides;

    [SerializeField] private TileDatabase tileData;

    private Pathfinder<GridNode> pathfinder;

    [SerializeField] private DebugSettings debugSettings;

    private Vector3[] gizmoDirections = new Vector3[8]{
            new Vector3(0,1,0),
            new Vector3(0.7f,0.7f,0),
            new Vector3(1,0,0),
            new Vector3(0.7f,-0.7f,0),
            new Vector3(0,-1,0),
            new Vector3(-0.7f,-0.7f,0),
            new Vector3(-1,0,0),
            new Vector3(-0.7f,0.7f,0),
        };

    // INITIALISATION METHODS *********************************************************************
    public void Initialise()
    {
        tileData.Init();

        if (gridInfo.Length <= 0)
        {
            UnityEngine.Debug.LogError("no grids available");
            return;
        }

        int count = 0;
        foreach (var grid in gridInfo)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            InitGrid(grid, count);

            sw.Stop();
            if (debugSettings.showGridGenerationTime)
                JUtils.ShowTime(sw.ElapsedTicks, "grid [" + count.ToString() + "] initialized in:");

            count++;
        }

        pathfinder = new Pathfinder<GridNode>(Grid<GridNode>.CompositeGridArea(grids.ToArray()));

        foreach (var link in nodeOverrides.gridLinks)
        {
            GridNode node1 = grids[link.grid1.index][link.grid1.position];
            GridNode node2 = grids[link.grid2.index][link.grid2.position];

            Vector2Int grid1Offset = Vector2Int.RoundToInt( link.grid1.direction.IndexToRotation().Rotate(90));
            Vector2Int grid2Offset = Vector2Int.RoundToInt( link.grid2.direction.IndexToRotation().Rotate(-90));

            for (int i = 0; i < link.width; i++)
            {
                CreateGridLinkingNode(node1, node2, link.grid1, link.grid2.index, i, grid1Offset, grid2Offset);
                CreateGridLinkingNode(node2, node1, link.grid2, link.grid1.index, i, grid2Offset, grid1Offset);
            }
        }

        foreach (var link in nodeOverrides.sceneLinks)
        {
            Vector2Int gridOffset = Vector2Int.RoundToInt( link.travelDirection.IndexToRotation().Rotate(90));

            for (int i = 0; i < link.width; i++)
            {
                CreateSceneLinkingNode(link, gridOffset, i);
            }
        }
    }

    private void CreateGridLinkingNode(GridNode node, GridNode partner, LinkID link, int otherIndex, int i, Vector2Int offset, Vector2Int otherOffset)
    {
        GridNode thisNode = node;
        GridNode partnerNode = partner;
        if (i > 0)
        {
            thisNode = grids[link.index].GetNodeRelative(node.position, offset * i);
            partner = grids[otherIndex].GetNodeRelative(partner.position, otherOffset * i);
        }
        

        thisNode.overridden = true;
        thisNode.overriddenDir = link.direction;
        thisNode.Neighbors[link.direction].connected = true;
        thisNode.Neighbors[link.direction].oneway = false;
        thisNode.Neighbors[link.direction].overridden = true;
        thisNode.Neighbors[link.direction].reference = partner;
    }

    private void CreateSceneLinkingNode(LevelTransitionInformation link, Vector2Int gridOffset, int i)
    {
        GridNode node = grids[link.myRoomIndex][link.myNodeIndex + (gridOffset * i)];
        if (node == null)
            return;
        node.overridden = true;
        node.overriddenDir = link.travelDirection;
        node.lvlTransitionInfo = link;

        GridNode transitionNode = new GridNode();
        transitionNode.position = new GridNodePosition(node.position);
        transitionNode.position.grid = new Vector2Int(int.MaxValue, int.MaxValue);
        transitionNode.position.world = transitionNode.position.world + new Vector3(link.getTravelDirection().x, link.getTravelDirection().y, transitionNode.position.world.z);
        transitionNode.overridden = true;
        transitionNode.overriddenDir = (-(link.getTravelDirection())).RotationToIndex(45);
        transitionNode.walkable = true;
        transitionNode.Neighbors = new NodeNeighborhood<GridNode>(8);
        transitionNode.roomIndex = link.myRoomIndex;
        transitionNode.lvlTransitionInfo = new LevelTransitionInformation(link);
        transitionNode.lvlTransitionInfo.offsetIndex = (link.width-1) - i;
        transitionNode.lvlTransitionInfo.offsetVector = gridOffset;
        transitionNode.overrideType = NodeOverrideType.SceneConnection;

        int negativeDir = (-(link.getTravelDirection())).RotationToIndex(45);

        node.Neighbors[link.travelDirection].reference = transitionNode;
        node.Neighbors[link.travelDirection].connected = true;
        node.Neighbors[link.travelDirection].overridden = true;
        node.Neighbors[link.travelDirection].offsetVector = link.getTravelDirection();

        transitionNode.Neighbors[negativeDir].reference = node;
        transitionNode.Neighbors[negativeDir].connected = true;
        transitionNode.Neighbors[negativeDir].overridden = true;
        transitionNode.Neighbors[negativeDir].offsetVector = -link.getTravelDirection();

        nodeOverrides.sceneTransitionNodes.Add(transitionNode);
    }

    // GRID INITIALISATION METHODS ****************************************************************
    //private void InitGrid(Grid<GridNode> grid)
    private void InitGrid(GridInfo gridInfo, int index)
    {
        Grid<GridNode> grid = new Grid<GridNode>(
                gridInfo.width,
                gridInfo.height,
                gridInfo.cellSize,
                gridInfo.originPosition
                );

        grid.Init();

        // TODO: doing this in two loops is kinda yuck, there is probably a better way of doing this.
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                CreateNode(x, y, grid, index);
            }
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                PreCalculateNeighbours(x, y, grid);
            }
        }

        grids.Add(grid);
    }

    // after building and populating the grid with nodes, calculate all neighbouring node links
    // and attach them to each node
    private void PreCalculateNeighbours(int x, int y, Grid<GridNode> grid)
    {
        foreach (NodeNeighbor<GridNode> neighbor in grid[x, y].Neighbors)
        {
            // if node is not supposed to have neighbour in this direction
            if (neighbor.offsetVector == Vector2.zero)
                continue;

            GridNode neighbourNode = grid.GetNodeRelative(
                    x,
                    y,
                    Mathf.RoundToInt(neighbor.offsetVector.x),
                    Mathf.RoundToInt(neighbor.offsetVector.y)
                    );

            if (neighbourNode == null)
            {
                neighbor.connected = false;
                continue;
            }

            if (neighbourNode.walkable != grid[x, y].walkable)
            {
                neighbor.connected = false;
                continue;
            }

            bool matching = false;
            bool neighborIsOneway = false;

            // TODO: this next stuff is kinda gross.. there is probably a better way of doing this.

            // Search through all neighbour directions to find this nodes opposite (so both
            // neighbour vectors point at each other) and check that both these connections are active.
            // figure out whether or not the node should be connected based on whether or not
            // both connections are/aren't oneway connections.
            foreach (NodeNeighbor<GridNode> newNeighbor in neighbourNode.Neighbors)
            {
                if (neighbor.offsetVector == newNeighbor.offsetVector * -1)
                {
                    if (neighbor.connected && newNeighbor.connected)
                    {
                        matching = true;
                        neighbor.connected = true;
                        if (newNeighbor.oneway)
                        {
                            matching = false;
                            neighborIsOneway = true;
                        }

                        if (neighbor.oneway)
                            newNeighbor.connected = false;

                        break;
                    }

                    break;
                }
            }

            if (matching == true)
                neighbor.reference = neighbourNode;

            if (neighbor.reference == null && !neighborIsOneway)
                neighbor.connected = false;
        }
    }

    private void SetNeighborVectors(GridNode node, TileDataObject tileDataObject = null)
    {
        float angle = 0;
        float addition = 360 / 8;

        if (tileDataObject != null)
        {
            for (int i = 0; i < node.Neighbors.neighbors.Length; i++)
            {
                node.Neighbors[i].connected = (tileDataObject.neighbours[i] != STATE.OFF);
                node.Neighbors[i].oneway = (tileDataObject.neighbours[i] == STATE.ONEWAY);

                Vector2 direction = Vector2.up.Rotate(angle);
                if (node.Neighbors[i].connected)
                    node.Neighbors[i].offsetVector = direction;

                angle += addition;
            }
        }
    }

    private void CreateNode(int x, int y, Grid<GridNode> grid, int index)
    {
        bool walkable = false;
        int tilecount = 0;
        TileDataObject tileDataObject = null;

        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        foreach (Tilemap tilemap in tileData.tilemaps)
        {
            Vector3Int currentTile = tilemap.WorldToCell(grid.ToWorld(x, y));

            if (tilemap.HasTile(currentTile))
            {
                if (tileData.TileHasData(tilemap, currentTile))
                {
                    tileDataObject = tileData[tilemap.GetTile(currentTile)];
                    tileDataObject.data.GetDataBool("walkable", out walkable);
                }
                tilecount++;
            }
        }

        //sw.Stop();
        //JUtils.ShowTime(sw.ElapsedTicks, "grid [" + index.ToString() + "] node[" + x + "," + y +"] tile data obtained in:");

        //Stopwatch sw2 = new Stopwatch();
        //sw2.Start();

        grid[x, y] = new GridNode();
        grid[x, y].position = grid.GetNodePosition(x, y);
        grid[x, y].overridden = false;
        grid[x, y].walkable = walkable;
        grid[x, y].Neighbors = new NodeNeighborhood<GridNode>(8);
        grid[x, y].roomIndex = index;

        if (tilecount > 0)
            SetNeighborVectors(grid[x, y], tileDataObject);

        //sw2.Stop();
        //JUtils.ShowTime(sw2.ElapsedTicks, "grid [" + index.ToString() + "] node[" + x + "," + y + "] created in:");
    }

    // DEBUG DRAWING METHODS **********************************************************************
    public void DrawGizmos()
    {
        if (grids == null)
            return;

        foreach (var grid in grids)
        {
            if (debugSettings.drawGrid)
                grid.DrawGizmos(debugSettings.drawGridColour, debugSettings.drawGridOutlineColour);

            if (debugSettings.drawNodes)
                DrawNodes(grid);
        }
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            foreach (var gridI in gridInfo)
            {
                if (debugSettings.drawGrid)
                    gridI.DrawGizmos(debugSettings.drawGridColour, debugSettings.drawGridOutlineColour);
            }
        }

        foreach (var link in nodeOverrides.gridLinks)
        {
            Gizmos.color = Color.yellow;

            if (gridInfo.Length < link.grid1.index || gridInfo.Length < link.grid2.index)
                continue;

            if (debugSettings.drawOverWrittenNodes)
            {
                Vector2 grid1Offset_2 = link.grid1.direction.IndexToRotation().Rotate(90);
                Vector2 grid2Offset_2 = link.grid2.direction.IndexToRotation().Rotate(-90);

                Vector3 grid1Offset = new Vector3(grid1Offset_2.x, grid1Offset_2.y, 0);
                Vector3 grid2Offset = new Vector3(grid2Offset_2.x, grid2Offset_2.y, 0);

                for (int i = 0; i < link.width; i++)
                {
                    Gizmos.DrawSphere(
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position) + (grid1Offset*i),
                        gridInfo[link.grid1.index].cellSize / 8
                        );
                    Gizmos.DrawSphere(
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position) + (grid2Offset * i),
                        gridInfo[link.grid2.index].cellSize / 8
                        );

                    Gizmos.DrawRay(
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position) + (grid1Offset * i),
                        gizmoDirections[link.grid1.direction] * 0.25f
                        );

                    Gizmos.DrawRay(
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position) + (grid2Offset * i),
                        gizmoDirections[link.grid2.direction] * 0.25f
                        );

                    Gizmos.DrawLine(
                        (gridInfo[link.grid1.index].ToWorld(link.grid1.position) + (grid1Offset * i)) + (gizmoDirections[link.grid1.direction] * 0.25f),
                        (gridInfo[link.grid2.index].ToWorld(link.grid2.position) + (grid2Offset * i)) + (gizmoDirections[link.grid2.direction] * 0.25f)
                        );
                }


            }
        }

        int count = 0;
        foreach (var link in nodeOverrides.sceneLinks)
        {
            Gizmos.color = Color.magenta;

            if (gridInfo.Length < link.myRoomIndex)
            {
                count++;
                continue;
            }

            Vector2 gridOffset_2 = link.travelDirection.IndexToRotation().Rotate(90);

            Vector3 gridOffset = new Vector3(gridOffset_2.x, gridOffset_2.y, 0);

            if (debugSettings.drawOverWrittenNodes)
            {
                for (int i = 0; i < link.width; i++)
                {
                    Gizmos.DrawSphere(
                        gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex) + (gridOffset * i),
                        gridInfo[link.myRoomIndex].cellSize / 8
                        );

                    if (Application.isPlaying)
                    {
                        Gizmos.DrawSphere(
                            nodeOverrides.sceneTransitionNodes[count].position.world + (gridOffset * i),
                            gridInfo[link.myRoomIndex].cellSize / 8
                            );
                    }

                    Gizmos.DrawRay(
                        gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex) + (gridOffset * i),
                        gizmoDirections[link.travelDirection] * 0.25f
                        );
                }
            }

            count++;
        }
#endif
    }

    private void DrawNodes(Grid<GridNode> grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                DrawNode(x, y, grid);
            }
        }
    }

    private void DrawNode(int x, int y, Grid<GridNode> grid)
    {
        Gizmos.color = new Color(1, 1, 1, 0.25f);

        if (grid.NodeExists(x, y))
        {
            Gizmos.color = (grid[x, y].overridden) ? Color.yellow : (grid[x, y].walkable) ? Color.blue : Color.red;
        }

        Gizmos.DrawSphere(grid.ToWorld(x, y), 1 * (grid.CellSize / 8));

        if (debugSettings.drawNodeConnections && grid.NodeExists(x, y))
        {
            foreach (NodeNeighbor<GridNode> neighbor in grid[x, y].Neighbors)
            {
                if (!neighbor.connected && !neighbor.overridden)
                    continue;

                if (neighbor.reference == null && !neighbor.overridden)
                    continue;

                Gizmos.color = (neighbor.oneway) ? Color.red : Color.blue;

                if (neighbor.overridden)
                    Gizmos.color = Color.yellow;

                Gizmos.DrawLine(
                    grid.ToWorld(x, y),
                    grid.ToWorld(x, y) + (new Vector3(neighbor.offsetVector.x, neighbor.offsetVector.y, grid.ToWorld(x, y).z) * (grid.CellSize / 2))
                    );
            }
        }
    }

    // PATHFINDING METHODS ************************************************************************
    public Vector3[] GetPath(int x, int y, int end_x, int end_y) => GetPath(grids[0].ToWorld(x, y), grids[0].ToWorld(end_x, end_y));

    public Vector3[] GetPath(Vector2Int start, Vector2Int end) => GetPath(grids[0].ToWorld(start.x, start.y), grids[0].ToWorld(end.x, end.y));

    public Vector3[] GetPath(Vector3 start, Vector3 end)
    {
        GridNode startNode = GetNodeFromWorld(start);
        GridNode endNode = GetNodeFromWorld(end);

        //if(!grids[0].NodeExistsAt(start) || !grids[0].NodeExistsAt(end))
        if (startNode == null || endNode == null)
        {
            UnityEngine.Debug.LogWarning("trying to pathfind to non existent nodes");
            return null;
        }

        return pathfinder.FindPath(startNode, endNode, debugSettings.showPathfindTime);
    }

    // MISC METHODS *******************************************************************************
    public Grid<GridNode> Grid(int i)
    {
        return grids[i];
    }

    public GridNode GetNodeFromWorld(Vector3 wpos)
    {
        bool nodeExists = false;
        int i;

        for (i = 0; i < grids.Count; i++)
        {
            nodeExists = grids[i].NodeExistsAt(wpos);

            if (nodeExists)
                return grids[i].WorldToNode(wpos);
        }

        return null;
    }
}

// TILE DATABASE ************************************************************************************************************************************
[System.Serializable]
public class TileDatabase
{
    [SerializeField] private List<TileDataObject> tileData;
    [SerializeField] public List<Tilemap> tilemaps;

    private Dictionary<TileBase, TileDataObject> dataFromTiles;

    public TileDataObject this[TileBase t]
    {
        get { return dataFromTiles[t]; }
    }

    public void Init()
    {
        dataFromTiles = new Dictionary<TileBase, TileDataObject>();

        foreach (var _tileData in tileData)
        {
            foreach (var tile in _tileData.tiles)
            {
                dataFromTiles.Add(tile, _tileData);
            }
        }
    }

    public bool TileHasData(Tilemap map, Vector3Int currentCell)
    {
        TileBase currentTile = map.GetTile(currentCell);
        if (currentTile != null && dataFromTiles.ContainsKey(currentTile))
            return true;
        return false;
    }
}

// GRID DEBUG SETTINGS CLASS ********************************************************************************************************************
[System.Serializable]
public class DebugSettings
{
    [Header("Gizmo Settings")]
    public bool drawGrid;

    public Color drawGridOutlineColour;
    public Color drawGridColour;

    [Space(5)]
    public bool drawNodes;

    public bool drawOverWrittenNodes;

    public bool drawNodeConnections;

    [Header("Performance Testing Settings")]
    public bool showPathfindTime;

    public bool showGridGenerationTime;

    public DebugSettings()
    {
        drawGrid = false;
        drawGridOutlineColour = Color.white;
        drawGridColour = new Color(1, 1, 1, 0.1f);
    }
}

// GRIDINFO CLASS *******************************************************************************************************************************
[System.Serializable]
public class GridInfo
{
    [SerializeField, Min(1)] public int width = 2;
    [SerializeField, Min(1)] public int height = 2;
    [SerializeField, Min(0)] public float cellSize = 1;
    [SerializeField, Min(0)] public float cameraPadding = 1.25f;
    [SerializeField] public Vector3 originPosition = Vector3.zero;

    public Vector3 ToWorld(Vector2Int pos) => ToWorld(pos.x, pos.y);

    virtual public Vector3 ToWorld(int x, int y)
    {
        Vector3 pos = originPosition;

        if (x >= 0 && y >= 0 && x < width && y < height)
            pos = new Vector3(originPosition.x + x * cellSize, originPosition.y + y * cellSize, originPosition.z);

        return new Vector3(pos.x + (cellSize / 2), pos.y + (cellSize / 2), pos.z);
    }

    public void DrawGizmos(Color drawGridColour, Color drawGridOutlineColour)
    {
        Gizmos.color = drawGridOutlineColour;
        Vector3 startPos = new Vector3(originPosition.x, originPosition.y, originPosition.z);
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + height * cellSize, originPosition.z);
        Gizmos.DrawLine(startPos, targetPos);

        Gizmos.color = drawGridColour;
        for (int i = 1; i < width; i++)
            Gizmos.DrawLine(startPos + (Vector3.right * i * cellSize), targetPos + (Vector3.right * i * cellSize));
        Gizmos.color = drawGridOutlineColour;
        Gizmos.DrawLine(startPos + (Vector3.right * width * cellSize), targetPos + (Vector3.right * width * cellSize));

        targetPos = new Vector3(startPos.x + width * cellSize, startPos.y, originPosition.z);
        Gizmos.DrawLine(startPos, targetPos);

        Gizmos.color = drawGridColour;
        for (int i = 1; i < height; i++)
            Gizmos.DrawLine(startPos + (Vector3.up * i * cellSize), targetPos + (Vector3.up * i * cellSize));
        Gizmos.color = drawGridOutlineColour;
        Gizmos.DrawLine(startPos + (Vector3.up * height * cellSize), targetPos + (Vector3.up * height * cellSize));
    }
}

// GRID NODE OVERRIDER CLASS ********************************************************************************************************************
[System.Serializable]
public class NodeOverrides<GridNode>
{
    [SerializeField] public GridLink[] gridLinks;

    [SerializeField] public List<LevelTransitionInformation> sceneLinks;

    [HideInInspector] public List<GridNode> sceneTransitionNodes;
}

// INTER-GRID LINKS *****************************************************************************************************************************
[System.Serializable]
public struct GridLink
{
    public int width;
    public LinkID grid1;
    public LinkID grid2;

    public GridLink(int w)
    {
        width = 1;
        grid1 = new LinkID();
        grid2 = new LinkID();
    }
}

[System.Serializable]
public struct LinkID
{
    public int index;
    public Vector2Int position;
    [Range(0, 7)] public int direction;
}

// LEVEL TRANSITION INFORMATION STRUCT **********************************************************************************************************
[System.Serializable]
public class LevelTransitionInformation
{
    [SerializeField] public int width;

    [Header("Node information")]
    [SerializeField] public int myRoomIndex;
    [SerializeField] public Vector2Int myNodeIndex;

    [Header("Transition Information")]
    [SerializeField] public string targetSceneName;

    [SerializeField] public int targetRoomIndex;
    [SerializeField] public Vector2Int targetNodeIndex;
    [SerializeField, HideInInspector] public Vector2Int offsetVector;
    [SerializeField, HideInInspector] public int offsetIndex;
    [Range(0, 7), SerializeField] private int m_travelDirection;
    [SerializeField] public blu.TransitionType transitionType;
    [SerializeField] public blu.LoadingBarType loadType;

    public LevelTransitionInformation(LevelTransitionInformation in_lvlInfo)
    {
        myRoomIndex         = in_lvlInfo.myRoomIndex;
        myNodeIndex         = in_lvlInfo.myNodeIndex;

        targetSceneName     = in_lvlInfo.targetSceneName;
        targetRoomIndex     = in_lvlInfo.targetRoomIndex;
        targetNodeIndex     = in_lvlInfo.targetNodeIndex;

        offsetVector        = in_lvlInfo.offsetVector;
        offsetIndex         = in_lvlInfo.offsetIndex;
        m_travelDirection   = in_lvlInfo.travelDirection;

        transitionType      = in_lvlInfo.transitionType;
        loadType            = in_lvlInfo.loadType;
    }

    public int travelDirection
    { get { return m_travelDirection; } set { m_travelDirection = value; } }

    public void SetTravelDirection(Vector2 vec) => m_travelDirection = vec.RotationToIndex();

    public Vector2 getTravelDirection()
    {
        return Vector2.up.Rotate(m_travelDirection * 45);
    }
}

// MULTIGRID NODE INTERFACE *********************************************************************************************************************
public interface MultiNode
{
    public int roomIndex { get; set; }

    public bool walkable { get; set; }
    public bool overridden { get; set; }
    public int overriddenDir { get; set; }

    public NodeOverrideType overrideType { get; set; }

    public LevelTransitionInformation lvlTransitionInfo { get; set; }

    public int lvlTransitionIndexOffset { get; set; }
}

public enum NodeOverrideType
{
    None = 0,
    RoomConnection,
    SceneConnection
}