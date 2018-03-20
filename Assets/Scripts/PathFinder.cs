using UnityEngine;
using System.Collections.Generic;

public class PathFinder {

    public float D = 1; // The heuristic constant applied on computation for a minimum normal cost
    public float D2 = Mathf.Sqrt(2); // Cost for diagonal heuristics (sqrt(2) -> octile distance)

    Node Start;
    Node End;

    Node[,] FullList;
    List<Node> Closed = new List<Node>();
    List<Node> Opened = new List<Node>();

    public PathFinder(Node start, Node end, ref Node[,] nodes) {
        Opened.Add(start);
        Start = start;
        End = end;
        FullList = nodes;
    }

    public Node[] Compute() {
        return new Node[5];
    }

    // ===========================
    //         Heuristics
    // ===========================

    private double ManathanDistance(Node nodeA, Node nodeB) {
        double distX = nodeA.GridIndexes.x - nodeB.GridIndexes.x;
        double distY = nodeA.GridIndexes.y - nodeB.GridIndexes.y;

        return D * (distX * distY);
    }


    private double DiagManathan(Node nodeA, Node nodeB) {
        float distX = nodeA.GridIndexes.x - nodeB.GridIndexes.x;
        float distY = nodeA.GridIndexes.y - nodeB.GridIndexes.y;

        return D * (distX + distY) + (D2 - 2 * D) * Mathf.Min(distX, distY);
    }
}
