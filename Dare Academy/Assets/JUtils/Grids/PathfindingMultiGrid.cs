using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using UnityEditor;

namespace JUtil.Grids
{
    // PATHFINDING WITH MULTIPLE GRIDS CLASS ************************************************************************************************************
    [System.Serializable]
    public class PathfindingMultiGrid<T>
        where T : class, IPathFindingNode<T>, IHeapItem<T>, MultiNode, new()
    {
        // MEMBERS ************************************************************************************
        [SerializeField] private List<Grid<T>> grids;

        [SerializeField] private GridInfo[] gridInfo;

        [SerializeField] private string[] gridNames;

        [SerializeField] private NodeOverrides<T> nodeOverrides;

        [SerializeField] private TileDatabase tileData;

        private Pathfinder<T> pathfinder;

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

            pathfinder = new Pathfinder<T>(Grid<T>.CompositeGridArea(grids.ToArray()));

            foreach (var link in nodeOverrides.gridLinks)
            {
                T node1 = grids[link.grid1.index][link.grid1.position];
                T node2 = grids[link.grid2.index][link.grid2.position];

                CreateGridLinkingNode(node1, node2, link.grid1);
                CreateGridLinkingNode(node2, node1, link.grid2);
            }

            foreach (var link in nodeOverrides.sceneLinks)
            {
                CreateSceneLinkingNode(link);
            }
        }

        private void CreateGridLinkingNode(T node, T partner, LinkID link)
        {
            node.overridden = true;
            node.overriddenDir = link.direction;
            node.Neighbors[link.direction].connected = true;
            node.Neighbors[link.direction].oneway = false;
            node.Neighbors[link.direction].overridden = true;
            node.Neighbors[link.direction].reference = partner;
        }

        private void CreateSceneLinkingNode(LevelTransitionInformation link)
        {
            T node                  = grids[link.myRoomIndex][link.myNodeIndex];
            node.overridden         = true;
            node.overriddenDir      = link.travelDirection;
            node.lvlTransitionInfo  = link;

            T transitionNode                    = new T();
            transitionNode.position             = new GridNodePosition(node.position);
            transitionNode.position.grid        = new Vector2Int(int.MaxValue, int.MaxValue);
            transitionNode.position.world       = transitionNode.position.world + new Vector3(link.getTravelDirection().x, link.getTravelDirection().y, transitionNode.position.world.z);
            transitionNode.overridden           = true;
            transitionNode.overriddenDir        = (-(link.getTravelDirection())).RotationToIndex(45);
            transitionNode.walkable             = true;
            transitionNode.Neighbors            = new NodeNeighborhood<T>(8);
            transitionNode.roomIndex            = link.myRoomIndex;
            transitionNode.lvlTransitionInfo    = link;
            transitionNode.overrideType         = NodeOverrideType.SceneConnection;

            int negativeDir = (-(link.getTravelDirection())).RotationToIndex(45);

            node.Neighbors[link.travelDirection].reference      = transitionNode;
            node.Neighbors[link.travelDirection].connected      = true;
            node.Neighbors[link.travelDirection].overridden     = true;
            node.Neighbors[link.travelDirection].offsetVector   = link.getTravelDirection();

            transitionNode.Neighbors[negativeDir].reference     = node;
            transitionNode.Neighbors[negativeDir].connected     = true;
            transitionNode.Neighbors[negativeDir].overridden    = true;
            transitionNode.Neighbors[negativeDir].offsetVector  = -link.getTravelDirection();

            nodeOverrides.sceneTransitionNodes.Add(transitionNode);
        }

        // GRID INITIALISATION METHODS ****************************************************************
        //private void InitGrid(Grid<T> grid)
        private void InitGrid(GridInfo gridInfo, int index)
        {
            Grid<T> grid = new Grid<T>(
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
        private void PreCalculateNeighbours(int x, int y, Grid<T> grid)
        {
            foreach (NodeNeighbor<T> neighbor in grid[x, y].Neighbors)
            {
                // if node is not supposed to have neighbour in this direction
                if (neighbor.offsetVector == Vector2.zero)
                    continue;

                T neighbourNode = grid.GetNodeRelative(
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
                foreach (NodeNeighbor<T> newNeighbor in neighbourNode.Neighbors)
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

        private void SetNeighborVectors(T node, TileDataObject tileDataObject = null)
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

        private void CreateNode(int x, int y, Grid<T> grid, int index)
        {
            bool walkable = false;
            int tilecount = 0;
            TileDataObject tileDataObject = null;

            int i = 0;
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

                i++;
            }

            grid[x, y]              = new T();
            grid[x, y].position     = grid.GetNodePosition(x, y);
            grid[x, y].overridden   = false;
            grid[x, y].walkable     = walkable;
            grid[x, y].Neighbors    = new NodeNeighborhood<T>(8);
            grid[x, y].roomIndex    = index;

            if (tilecount > 0)
                SetNeighborVectors(grid[x, y], tileDataObject);
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

                if (!Application.isPlaying && debugSettings.drawOverWrittenNodes)
                {
                    Gizmos.DrawSphere(
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position),
                        gridInfo[link.grid1.index].cellSize / 8
                        );
                    Gizmos.DrawSphere(
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position),
                        gridInfo[link.grid2.index].cellSize / 8
                        );
                }

                if (debugSettings.drawOverWrittenNodes)
                {
                    Gizmos.DrawLine(
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position),
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position) + (gizmoDirections[link.grid1.direction] * 0.25f)
                        );

                    Gizmos.DrawLine(
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position),
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position) + (gizmoDirections[link.grid2.direction] * 0.25f)
                        );
                }

                if (debugSettings.drawNodeConnections && debugSettings.drawOverWrittenNodes)
                    Gizmos.DrawLine(
                        gridInfo[link.grid1.index].ToWorld(link.grid1.position) + (gizmoDirections[link.grid1.direction] * 0.25f),
                        gridInfo[link.grid2.index].ToWorld(link.grid2.position) + (gizmoDirections[link.grid2.direction] * 0.25f)
                        );
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
                    

                if (debugSettings.drawOverWrittenNodes)
                {
                    Gizmos.DrawSphere(
                        gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex),
                        gridInfo[link.myRoomIndex].cellSize / 8
                        );

                    if (EditorApplication.isPlaying)
                    {
                        Gizmos.DrawSphere(
                            nodeOverrides.sceneTransitionNodes[count].position.world,
                            gridInfo[link.myRoomIndex].cellSize / 8
                            );
                    }
                }

                if (debugSettings.drawOverWrittenNodes)
                {
                    Gizmos.DrawLine(
                        gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex),
                        gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex) + (gizmoDirections[link.travelDirection] * 0.25f)
                        );
                }

                count++;
            }
#endif
        }

        private void DrawNodes(Grid<T> grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    DrawNode(x, y, grid);
                }
            }
        }

        private void DrawNode(int x, int y, Grid<T> grid)
        {
            Gizmos.color = new Color(1, 1, 1, 0.25f);

            if (grid.NodeExists(x, y))
            {
                Gizmos.color = (grid[x, y].overridden) ? Color.yellow : (grid[x, y].walkable) ? Color.blue : Color.red;
            }

            Gizmos.DrawSphere(grid.ToWorld(x, y), 1 * (grid.CellSize / 8));

            if (debugSettings.drawNodeConnections && grid.NodeExists(x, y))
            {
                foreach (NodeNeighbor<T> neighbor in grid[x, y].Neighbors)
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
            T startNode = GetNodeFromWorld(start);
            T endNode = GetNodeFromWorld(end);

            //if(!grids[0].NodeExistsAt(start) || !grids[0].NodeExistsAt(end))
            if (startNode == null || endNode == null)
            {
                UnityEngine.Debug.LogWarning("trying to pathfind to non existent nodes");
                return null;
            }

            return pathfinder.FindPath(startNode, endNode, debugSettings.showPathfindTime);
        }

        // MISC METHODS *******************************************************************************
        public Grid<T> Grid(int i)
        {
            return grids[i];
        }

        public T GetNodeFromWorld(Vector3 wpos)
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
    public class NodeOverrides<T>
    {
        [SerializeField] public GridLink[] gridLinks;

        [SerializeField] public List<LevelTransitionInformation> sceneLinks;

        [HideInInspector] public List<T> sceneTransitionNodes;
    }

    // INTER-GRID LINKS *****************************************************************************************************************************
    [System.Serializable]
    public struct GridLink
    {
        public LinkID grid1;
        public LinkID grid2;
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
        [Header("Node information")]
        [SerializeField] public int myRoomIndex;
        [SerializeField] public Vector2Int myNodeIndex;

        [Header("Transition Information")]
        [SerializeField] public string targetSceneName;
        [SerializeField] public int targetRoomIndex;
        [SerializeField] public Vector2Int targetNodeIndex;
        [Range(0, 7), SerializeField] private int m_travelDirection;

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
    }

    public enum NodeOverrideType
    {
        None = 0,
        RoomConnection,
        SceneConnection
    }
}