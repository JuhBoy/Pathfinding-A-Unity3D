using UnityEngine;
using System.Collections.Generic;

public class PathFinder {

    private MGrid Grid;

    /// <summary>
    /// The heuristic constant applied on computation for a minimum normal cost
    /// </summary>
    public float D = 1;

    /// <summary>
    /// Cost for diagonal heuristics (sqrt(2) -> octile distance)
    /// </summary>
    public float D2 = Mathf.Sqrt(2);

    /// <summary>
    /// The starting point and target used to create the path on the given Grid.
    /// </summary>
    private Node Start;
    private Node End;

    public List<Node> AStarPath = new List<Node>();

    /// <summary>
    /// Node to node path, handle historisation during computing processing
    /// </summary>
    private Dictionary<Node, Node> nodesPath = new Dictionary<Node, Node>();

    /// <summary>
    /// Register all costs for the current processing
    /// </summary>
    private Dictionary<Node, double> NodeCost = new Dictionary<Node, double>();

    public PathFinder(Node start, Node end, ref Node[,] nodes, MGrid grid) {
        Start = start;
        End = end;
        Grid = grid;
    }

    /// <summary>
    /// Handle A* algorithme
    /// </summary>
    /// <returns>The compute.</returns>
    public Stack<Node> Compute() {
        Queue<Node> frontier = new Queue<Node>();
        frontier.Enqueue(Start);

        NodeCost[Start] = 0;
        nodesPath[Start] = Start;

        while (frontier.Count > 0) {
            Node current = frontier.Dequeue();
            Node[] neighbours = Grid.GetNeighbours(current);

            foreach (Node neighbour in neighbours) {
                double cost = NodeCost[current] + DiagManathanDistance(neighbour, current);
                if (!neighbour.IsWalkable) continue;

                if (End.Equals(neighbour)) {
                    frontier.Clear();
                    End.parent = current;
                    break;
                }

                if (!NodeCost.ContainsKey(neighbour) || cost < NodeCost[neighbour]) {
                    neighbour.GCost = cost;
                    neighbour.HCost = cost + DiagManathanDistance(neighbour, End);

                    NodeCost[neighbour] = cost;
                    nodesPath[neighbour] = current;

                    neighbour.parent = current;
                    frontier.Enqueue(neighbour);
                }
            }
        }

        Node n = End;
        Stack<Node> aStarPath = new Stack<Node>();

        while (n.parent != null) {
            AStarPath.Add(n.parent);
            aStarPath.Push(n.parent);
            n = n.parent;
        }
        aStarPath.Push(n.parent);

        AStarPath.Reverse();
        return aStarPath;
    }

    // ===========================
    //         Heuristics
    // ===========================

    /// <summary>
    /// Manathans the distance heuristique for quadra move. (north / east / south / west
    /// </summary>
    /// <returns>The distance.</returns>
    /// <param name="nodeA">Node a.</param>
    /// <param name="nodeB">Node b.</param>
    private double ManathanDistance(Node nodeA, Node nodeB) {
        double distX = Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x);
        double distY = Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y);

        return D * (distX * distY);
    }

    /// <summary>
    /// Diags the manathan distance heuristic. Handle 8 moves situation using squared algorithm. (NE , NO, SE, SO ...)
    /// </summary>
    /// <returns>The manathan distance.</returns>
    /// <param name="nodeA">Node a.</param>
    /// <param name="nodeB">Node b.</param>
    private double DiagManathanDistance(Node nodeA, Node nodeB) {
        float distX = Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x);
        float distY = Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y);

        return D * (distX + distY) + (D2 - 2 * D) * Mathf.Min(distX, distY);
    }
}
