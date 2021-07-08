using System.Collections.Generic;
using UnityEngine;

namespace JUtil.Grids
{
    // GENERIC GRID DATA CLASS **********************************************************************************************************************
    [System.Serializable]
    public class Grid<T>
    {
        // Grid Settings
        [SerializeField, Min(1)] private int width      = 2;
        [SerializeField, Min(1)] private int height     = 2;
        [SerializeField, Min(0)] private float cellSize = 1;
        [SerializeField] private Vector3 originPosition = Vector3.zero;

        // Public Getters and Setters
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public float CellSize { get { return cellSize; } }
        public Vector3 OriginPosition { get { return originPosition; } }

        // Grid Array
        private T[,] grid;

        // STATIC MEMBERS *************************************************************************

        public static int CompositeGridArea(Grid<T>[] grids)
        {
            int totalArea = 0;

            foreach(var grid in grids)
            {
                totalArea += grid.Area;
            }

            return totalArea;
        }

        // CONSTRUCTORS ***************************************************************************
        public Grid(int in_width, int in_height, float in_cellSize, Vector3 in_originPosition)
        {
            width           = in_width;
            height          = in_height;
            cellSize        = in_cellSize;
            originPosition  = in_originPosition;
        }

        public Grid()
        {
            width           = 2;
            height          = 2;
            cellSize        = 1f;
            originPosition  = Vector3.zero;
        }

        // OPERATOR OVERLOADS *********************************************************************
        public T this[int x, int y]
        {
            get { return GetNode(x,y); }
            set { grid[x, y] = value; }
        }

        public T this[Vector2Int pos]
        {
            get { return GetNode(pos); }
            set { grid[pos.x, pos.y] = value; }
        }

        public T this[GridNodePosition pos]
        {
            get { return GetNode(pos.grid); }
            set { grid[pos.grid.x, pos.grid.y] = value; }
        }

        // INITIALISATION METHODS *****************************************************************
        virtual public void Init()
        {
            grid = new T[width , height];
        }

        // NODE INFORMATION METHODS ***************************************************************
        public bool NodeExists(Vector2Int pos) => NodeExists(pos.x, pos.y);
        public bool NodeExists(int x, int y)
        {
            if (grid == null)
                return false;

            // the generic equivalent to a null check.
            // god damn almost always reference types >:[
            if (EqualityComparer<T>.Default.Equals(grid[x,y], default(T)))
                return false;

            return true;
        }

        virtual public bool NodeExistsAt(Vector3 pos)
        {
            if (
                pos.x < originPosition.x || pos.x >= width + originPosition.x ||    // x alignment
                pos.y < originPosition.y || pos.y >= height + originPosition.y ||   // y alignment
                Mathf.RoundToInt(pos.z) != originPosition.z                         // z alignment
                )
                return false;
            return true;
        }

        protected T GetNode(Vector2Int pos) => GetNode(pos.x, pos.y);
        virtual protected T GetNode(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                if (NodeExists(x, y))
                    return grid[x, y];
                return default(T);
            }
            Debug.LogWarning("attempting to get node outside of grid bounds");
            return default(T);
        }

        // COORDINATE SPACE CONVERSION METHODS ****************************************************
        public Vector2Int WorldToGrid(Vector3 pos) => WorldToGrid(pos.x,pos.y,pos.z);
        virtual public Vector2Int WorldToGrid(float x, float y, float z)
        {
            if (x < originPosition.x || x >= width+originPosition.x || y < originPosition.y || y >= height+originPosition.y)
                return Vector2Int.one * -1;

            x -= originPosition.x;
            y -= originPosition.y;

            int g_x = Mathf.FloorToInt(x / cellSize);
            int g_y = Mathf.FloorToInt(y / cellSize);

            return new Vector2Int(g_x, g_y);
        }

        public Vector3 ToWorld(Vector2Int pos) => ToWorld(pos.x, pos.y);
        virtual public Vector3 ToWorld(int x, int y)
        {
            Vector3 pos = ToCell(x, y);
            return new Vector3(pos.x + (cellSize / 2), pos.y + (cellSize / 2), pos.z);
        }

        public static Vector3 GridToWorld(Vector3 originPos, int width, int height, float cellsize, Vector2Int pos) => GridToWorld(originPos, width, height, cellsize, pos.x, pos.y);
        public static Vector3 GridToWorld(Vector3 originPos, int width, int height, float cellSize, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return new Vector3(originPos.x + x * cellSize, originPos.y + y * cellSize, originPos.z);
            Debug.LogWarning("attempting to receive node outside of grid bounds");
            return Vector3.zero;
        }
            

        public Vector3 ToCell(Vector2Int pos) => ToCell(pos.x, pos.y);
        virtual public Vector3 ToCell(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return new Vector3(originPosition.x + x * cellSize, originPosition.y + y * cellSize, originPosition.z);
            Debug.LogWarning("attempting to receive node outside of grid bounds");
            return Vector3.zero;
        }



        // MISC HELPER METHODS ********************************************************************
        virtual public GridNodePosition GetNodePosition(int x, int y)
        {
            GridNodePosition pos    = new GridNodePosition();
            pos.grid                = new Vector2Int(x, y);
            pos.world               = ToWorld(x, y);
            return pos;
        }


        public T GetNodeRelative(GridNodePosition pos, int x_offset, int y_offest)  => GetNodeRelative(pos.grid.x, pos.grid.y, x_offset, y_offest);
        public T GetNodeRelative(GridNodePosition pos, Vector2Int offset)           => GetNodeRelative(pos.grid.x, pos.grid.y, offset.x, offset.y);
        public T GetNodeRelative(Vector2Int initPos, int x_offset, int y_offest)    => GetNodeRelative(initPos.x, initPos.y, x_offset, y_offest);
        public T GetNodeRelative(int x, int y, Vector2Int offset)                   => GetNodeRelative(x, y, offset.x, offset.y);
        public T GetNodeRelative(Vector2Int initPos, Vector2Int offset)             => GetNodeRelative(initPos.x, initPos.y, offset.x, offset.y);
        virtual public T GetNodeRelative(int x, int y, int x_offset, int y_offest)
        {
            if (x+x_offset >= 0 && y+y_offest >= 0 && x+x_offset < width && y+y_offest < height)
                return grid[x + x_offset, y + y_offest];
            return default(T);
        }

        public T WorldToNode(float x, float y, float z) => WorldToNode(new Vector3(x, y, z));
        virtual public T WorldToNode(Vector3 pos) { return this[WorldToGrid(pos)]; }

        virtual public int Area { get { return width * height; } }

        virtual public void DrawGizmos(Color drawGridColour, Color drawGridOutlineColour)
        {
            Gizmos.color = drawGridOutlineColour;
            Vector3 startPos = new Vector3(originPosition.x, originPosition.y, originPosition.z);
            Vector3 targetPos = new Vector3(startPos.x, startPos.y + height*cellSize, originPosition.z);
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

    // GRID NODE POSITION DATA CLASS ****************************************************************************************************************
    [System.Serializable]
    public class GridNodePosition
    {
        public Vector3 world;
        public Vector2Int grid;

        public GridNodePosition()
        {
            world = Vector3.zero;
            grid = Vector2Int.zero;
        }

        public GridNodePosition(GridNodePosition copy)
        {
            world = copy.world;
            grid = copy.grid;
        }
    }


}
