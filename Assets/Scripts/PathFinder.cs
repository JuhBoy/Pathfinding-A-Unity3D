using UnityEngine;
using System.Collections.Generic;

public class PathFinder {

    public MGrid Grid;

    public float D = 1; // The heuristic constant applied on computation for a minimum normal cost
    public float D2 = Mathf.Sqrt(2); // Cost for diagonal heuristics (sqrt(2) -> octile distance)

    private Node Start;
    private Node End;

    public List<Node> AStarPath = new List<Node>();

    public PathFinder(Node start, Node end, ref Node[,] nodes, MGrid grid) {
        Start = start;
        End = end;
        Grid = grid;
    }

    Dictionary<Node, Node> nodesPath = new Dictionary<Node, Node>();
    Dictionary<Node, double> NodeCost = new Dictionary<Node, double>();

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

    private double ManathanDistance(Node nodeA, Node nodeB) {
        double distX = Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x);
        double distY = Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y);

        return D * (distX * distY);
    }


    private double DiagManathanDistance(Node nodeA, Node nodeB) {
        float distX = Mathf.Abs(nodeA.GridIndexes.x - nodeB.GridIndexes.x);
        float distY = Mathf.Abs(nodeA.GridIndexes.y - nodeB.GridIndexes.y);

        return D * (distX + distY) + (D2 - 2 * D) * Mathf.Min(distX, distY);
    }
}
