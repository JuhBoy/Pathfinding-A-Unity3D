using UnityEngine;
using System.Collections.Generic;

public class PathFinder {

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
}
