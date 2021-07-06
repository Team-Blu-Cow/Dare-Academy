using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using JUtil;

namespace JUtil.Grids
{
    // PATHFINDER ***********************************************************************************************************************************
    public class Pathfinder<T> where T : class, IPathFindingNode<T>, IHeapItem<T>
    {
        private Heap<T> openSet;
        private HashSet<T> closedSet;

        T startNode;
        T targetNode;

        public int area { set; private get; }

        public Pathfinder(int A)
        {
            area = A;
        }

        public Vector3[] FindPath(T startPos, T targetPos, bool eightDir = false, bool showtime = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] wayPoints;
            bool pathSuccess = false;

            startNode = startPos;
            targetNode = targetPos;

            if (startNode.IsTraversable() && targetNode.IsTraversable())
            {
                openSet = new Heap<T>(area);
                closedSet = new HashSet<T>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {

                    T currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        sw.Stop();

                        if(showtime)
                            JUtils.ShowTime(sw.ElapsedTicks, "Path found in:");

                        pathSuccess = true;
                        break;
                    }

                    if (eightDir)
                        CheckNeighborsMoore(currentNode);
                    else
                        CheckNeighborsVonNeuman(currentNode);
                }
            }
            if (pathSuccess)
            {
                wayPoints = RetracePath(startNode, targetNode);
                return wayPoints;
            }
            UnityEngine.Debug.Log("Path not found");
            return null;
        }

        T GetNeighbor(T node, NodeNeighbor<T> neighbour)
        {
            // TODO: this method is redundant
            T neighbourNode = neighbour.reference;
            return neighbourNode;
        }

        void CheckNeighborsMoore(T currentNode)
        {
            foreach (NodeNeighbor<T> neighbourStruct in currentNode.Neighbors)
            {
                T neighbour = GetNeighbor(currentNode, neighbourStruct);

                if (!neighbourStruct.connected || neighbour == null)
                    continue;

                if (!neighbour.IsTraversable() || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }

            }
        }

        void CheckNeighborsVonNeuman(T currentNode)
        {
            int count = -1;
            foreach (NodeNeighbor<T> neighbourStruct in currentNode.Neighbors)
            {
                count++;
                if (count % 2 != 0)
                {
                    continue;
                }

                T neighbour = GetNeighbor(currentNode, neighbourStruct);

                if (!neighbourStruct.connected || neighbour == null)
                    continue;

                if (!neighbour.IsTraversable() || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }


            }
        }

        Vector3[] RetracePath(T startNode, T endNode)
        {
            List<T> path = new List<T>();
            T currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Add(currentNode);
            Vector3[] waypoints = SimplifyPath(path);

            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<T> path)
        {
            List<Vector3> waypoints = new List<Vector3>();

            for (int i = 0; i < path.Count - 1; i++)
            {
                waypoints.Add(new Vector3(path[i].position.world.x, path[i].position.world.y, 2));
            }

            return waypoints.ToArray();
        }

        int GetDistance(T nodeA, T nodeB)
        {
            int dstX = Mathf.Abs(nodeA.position.grid.x - nodeB.position.grid.x);
            int dstY = Mathf.Abs(nodeA.position.grid.y - nodeB.position.grid.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }

    }

    // PATHFINDING NODE INTERFACE *******************************************************************************************************************
    public interface IPathFindingNode<T>
        where T : class
    {
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; }

        public NodeNeighborhood<T> Neighbors { get; set; }

        public T parent { get; set; }

        public GridNodePosition position { get; set; }

        public bool IsTraversable();
    }

    // NODE NEIGHBOURHOOD CONTAINER *****************************************************************************************************************
    [System.Serializable]
    public class NodeNeighborhood<T> : IEnumerable<NodeNeighbor<T>>
        where T : class
    {
        public NodeNeighbor<T>[] neighbors;

        public NodeNeighbor<T> this[int i]
        {
            get { return neighbors[i]; }
            set { neighbors[i] = value; }
        }

        public NodeNeighbor<T> this[Vector2 dir]
        {
            get 
            {
                if (dir == Vector2.zero)
                    return null;
                return neighbors[dir.RotationToIndex()];
            }
            set { neighbors[dir.RotationToIndex()] = value; }
        }

        public NodeNeighbor<T> this[Vector2 dir, int sliceAngle]
        {
            get 
            {
                if (dir == Vector2.zero)
                    return null;
                return neighbors[dir.RotationToIndex(sliceAngle)];
            }
            set { neighbors[dir.RotationToIndex(sliceAngle)] = value; }
        }

        public NodeNeighborhood(int count)
        {
            neighbors = new NodeNeighbor<T>[count];
            for (int i = 0; i < count; i++)
            {
                neighbors[i] = new NodeNeighbor<T>();
            }
        }

        public IEnumerator<NodeNeighbor<T>> GetEnumerator()
        {
            return ((IEnumerable<NodeNeighbor<T>>)neighbors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return neighbors.GetEnumerator();
        }

    }

    // NODE NEIGHBOUR "STRUCT" **********************************************************************************************************************
    public class NodeNeighbor<T>
    {
        public T reference;
        public bool connected;
        public bool oneway;
        public bool overridden;
        public Vector2 offsetVector;

        public NodeNeighbor()
        {
            reference = default(T);
            connected = true;
            offsetVector = Vector2.zero;
            oneway = false;
            overridden = false;
        }
    }
}
