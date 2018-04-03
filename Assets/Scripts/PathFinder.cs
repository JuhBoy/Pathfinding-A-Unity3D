using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public PathFinder(MGrid grid) {
        Grid = grid;
    }

    public Stack<Node> Compute(Node start, Node end) {
        List<Node> openList = new List<Node>();
        Stack<Node> closedList = new Stack<Node>();

        start.GCost = 0.0d;
        openList.Add(start);

        while (openList.Count > 0) {
            Node current;
            FindLowestFValue(openList, out current);

            openList.Remove(current);
            closedList.Push(current);

            foreach (Node neighbour in Grid.GetNeighbours(current)) {
                if (!neighbour.IsWalkable || closedList.Contains(neighbour)) { continue; }

                if (neighbour.Equals(end)) {
                    openList.Clear();
                    end.parent = current;
                    break;
                }

                if (!openList.Contains(neighbour)) {
                    neighbour.GCost = current.GCost + DiagManathanDistance(neighbour, current);
                    neighbour.HCost = DiagManathanDistance(neighbour, end);
                    neighbour.parent = current;
                    openList.Add(neighbour);
#if MYDEBUG
                    CreateBoxDebug(neighbour.WorldPosition, neighbour);
#endif
                } else {
                    double previousFscore = (double)neighbour.FCost;
                    double newFcost = (double)current.GCost + DiagManathanDistance(neighbour, current) + (double)neighbour.HCost;

                    if (newFcost < previousFscore) {
                        neighbour.GCost = current.GCost + DiagManathanDistance(neighbour, current);
                        neighbour.parent = current;
#if MYDEBUG
                        UpdateBoxDebug(neighbour);
#endif
                    }
                }
            }
        }

        return ComputePath(end);
    }

    private void FindLowestFValue(List<Node> list, out Node current) {
        double lowest = double.MaxValue;
        current = list[0];

        foreach (Node node in list) {
            if (node.FCost <= lowest) {
                lowest = (double)node.FCost;
                current = node;
            }
        }
    }

    private Stack<Node> ComputePath(Node end) {
        Node currentNode = end;
        Stack<Node> path = new Stack<Node>();
        path.Push(end);

        while (currentNode.parent != null) {
            Node temp = currentNode.parent;
            path.Push(temp);

            currentNode.parent = null;
            currentNode = temp;
        }

        return path;
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

        double cost = D * (distX + distY) + (D2 - 2 * D) * Mathf.Min(distX, distY);
        cost += nodeA.Modifier;

        return cost;
    }


#if MYDEBUG
    public List<GameObject> games = new List<GameObject>();

    private void CreateBoxDebug(Vector3 position, Node node) {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = position;
        var d = g.AddComponent<DebugCube>();
        d.hcost = node.HCost;
        d.fcost = (double)node.FCost;
        d.gcost = (double)node.GCost;
        d.parent = node.parent;
        games.Add(g);
    }

    private void UpdateBoxDebug(Node node) {
        DebugCube d = games.Find(n => n.transform.position == node.WorldPosition).GetComponent<DebugCube>();
        d.fcost = (double)node.FCost;
        d.gcost = (double)node.GCost;
        d.hcost = node.HCost;
        d.parent = node.parent;
    }

    public class DebugCube : MonoBehaviour {
        public double fcost;
        public double gcost;
        public double hcost;
        public Node parent;
    }
#endif
}


