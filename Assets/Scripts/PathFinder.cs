using System;
using UnityEngine;
using System.Collections.Generic;

public enum Heuristic {
    Manhattan,
    Euclidean,
    Chebyshev
}

public class PathFinder {
    private MGrid Grid;
    private Heuristic UsedHeuristic { get; }

    public PathFinder(MGrid grid, Heuristic heuristic) {
        Grid = grid;
        UsedHeuristic = heuristic;
    }

    public Stack<Node> Compute(Node start, Node end) {
        var openList = new List<Node>(8);
        var closedList = new Stack<Node>(Grid.Cols * Grid.Rows);

        start.GCost = 0.0d;
        openList.Add(start);

        while (openList.Count > 0) {
            PopLowestValue(openList, out Node current);
            closedList.Push(current);

            foreach (Node neighbour in Grid.GetNeighbours(current)) {
                if (!neighbour.IsWalkable || closedList.Contains(neighbour)) {
                    continue;
                }

                if (neighbour.Equals(end)) {
                    openList.Clear();
                    end.Parent = current;
                    break;
                }

                if (!openList.Contains(neighbour)) {
                    neighbour.GCost = current.GCost + Heuristic(neighbour, current);
                    neighbour.HCost = Heuristic(neighbour, end);
                    neighbour.Parent = current;
                    openList.Add(neighbour);
#if UNITY_EDITOR
                    CreateBoxDebug(neighbour.WorldPosition, neighbour);
#endif
                } else {
                    double newGCost = current.GCost + Heuristic(neighbour, current);
                    double newFCost = newGCost + neighbour.HCost;

                    if (newFCost < neighbour.FCost) {
                        neighbour.GCost = newGCost;
                        neighbour.Parent = current;
#if UNITY_EDITOR
                        UpdateBoxDebug(neighbour);
#endif
                    }
                }
            }
        }

        return ComputePath(end);
    }

    private static void PopLowestValue(IList<Node> list, out Node current) {
        var lowest = double.MaxValue;
        current = list[0];

        foreach (Node node in list) {
            if (node.FCost <= lowest) {
                lowest = node.FCost;
                current = node;
            }
        }

        list.Remove(current);
    }

    private static Stack<Node> ComputePath(Node end) {
        Node currentNode = end;
        var path = new Stack<Node>();
        path.Push(end);

        while (currentNode.Parent != null) {
            Node temp = currentNode.Parent;
            path.Push(temp);

            currentNode.Parent = null;
            currentNode = temp;
        }

        return path;
    }

    // ===========================
    //         Heuristics
    // ===========================

    private double Heuristic(Node nodeA, Node nodeB) {
        double result = UsedHeuristic switch {
            global::Heuristic.Manhattan => ManhattanDistance(nodeA, nodeB),
            global::Heuristic.Euclidean => EuclideanDistance(nodeA, nodeB),
            global::Heuristic.Chebyshev => ChebyshevDistance(nodeA, nodeB),
            _ => throw new ArgumentOutOfRangeException()
        };
        return result;
    }

    /// <summary>
    /// Manhattan Distance + terrain constraint. (north / east / south / west)
    /// </summary>
    private double ManhattanDistance(Node nodeA, Node nodeB) {
        double distX = Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x);
        double distY = Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y);
        return distX + distY + nodeA.Modifier;
    }

    /// <summary>
    /// Euclidean² distance + terrain constraint.
    /// </summary>
    private double EuclideanDistance(Node nodeA, Node nodeB) { return Vector3.Distance(nodeA.WorldPosition, nodeB.WorldPosition) + nodeA.Modifier; }

    /// <summary>
    /// Chebyshev distance (Chess distance) + terrain constraint.
    /// </summary>
    private static double ChebyshevDistance(Node nodeA, Node nodeB) {
        return Mathf.Max(Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x), Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y)) + nodeA.Modifier;
    }


#if UNITY_EDITOR
    private readonly List<GameObject> _debugObjectCache = new List<GameObject>();

    private void CreateBoxDebug(Vector3 position, Node node) {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = position;
        var d = g.AddComponent<DebugCube>();
        d.hcost = node.HCost;
        d.fcost = (double) node.FCost;
        d.gcost = (double) node.GCost;
        d.parent = node.Parent;
        _debugObjectCache.Add(g);
    }

    private void UpdateBoxDebug(Node node) {
        var debugCube = _debugObjectCache.Find(n => n.transform.position == node.WorldPosition).GetComponent<DebugCube>();
        debugCube.fcost = (double) node.FCost;
        debugCube.gcost = (double) node.GCost;
        debugCube.hcost = node.HCost;
        debugCube.parent = node.Parent;
    }

    public void ClearCache() {
        foreach (GameObject obj in _debugObjectCache) {
            UnityEngine.Object.Destroy(obj);
        }
        _debugObjectCache.Clear();
    }

    public class DebugCube : MonoBehaviour {
        public double fcost;
        public double gcost;
        public double hcost;
        public Node parent;
    }
#endif
}