using System;
using UnityEngine;

public class MGrid : MonoBehaviour {

    public Transform PlayerWorldPosition;
    public Transform EndPointWorldPositio;

    // =====================
    //       General
    // =====================
    public Vector2 GridSize;
    public int Rows, Cols;
    public Node[,] NodesList;
    public LayerMask UnwalkableMask;

    // =====================
    //         Sizes
    // =====================
    public int NodeDiamter, NodeRadius;
    public Vector3 BottomLeftWorldPoint;
    public Node PlayerGridPosition;
    public Node EndPointGridPosition;

    // =====================
    //       PathFinder
    // =====================
    public PathFinder PathFinder;
    public Node[] _orderedNodes;
    public bool JobRunning;

    public Node[] OrderedNodes { get { return _orderedNodes; } set { _orderedNodes = value; JobRunning = false; PathFinder = null; } }

    void Start() {
        NodeDiamter = NodeRadius * 2;
        Rows = Mathf.RoundToInt(GridSize.x / NodeDiamter);
        Cols = Mathf.RoundToInt(GridSize.y / NodeDiamter);

        FillNodes();
    }

	private void Update() {
        if (NodesList != null) {
            try {
                PlayerGridPosition = NodePositionFromWorldPosition(PlayerWorldPosition.transform.position);
                EndPointGridPosition = NodePositionFromWorldPosition(EndPointWorldPositio.transform.position);
            } catch (Exception e) {
                // if it's out
                Debug.Log("e:" + e);
            }
        }

        if (Input.GetKey(KeyCode.Space) && !JobRunning) {
            PathFinder = new PathFinder(PlayerGridPosition, EndPointGridPosition, ref NodesList);
            OrderedNodes = PathFinder.Compute();
            JobRunning = true;
        }
	}

	private void FillNodes() {
        NodesList = new Node[Rows, Cols];
        BottomLeftWorldPoint = transform.position - (Vector3.forward * GridSize.y / 2) - (Vector3.right * GridSize.x / 2) + ((Vector3.right + Vector3.forward) * NodeRadius);

        for (int r = 0; r < Rows; r++) {
            for (int c = 0; c < Cols; c++) {
                Vector3 worldPosition = new Vector3(
                    x: BottomLeftWorldPoint.x + (r * NodeDiamter),
                    y: 0,
                    z: BottomLeftWorldPoint.z + (c * NodeDiamter)
                );
                bool walkable = !Physics.CheckSphere(worldPosition, NodeRadius, UnwalkableMask);
                NodesList[r, c] = new Node(walkable, worldPosition, new Vector2Int(r, c));
            }
        }
    }

    private Node NodePositionFromWorldPosition(Vector3 worldPosition) {
        Vector2 percentPosition = new Vector2();

        percentPosition.x = ((worldPosition.x + (GridSize.x / 2 - 1)) - transform.position.x) / GridSize.x * Rows;
        percentPosition.y = ((worldPosition.z + (GridSize.y / 2 - 1)) - transform.position.z) / GridSize.y * Cols;

        return NodesList[Mathf.RoundToInt(percentPosition.x), Mathf.RoundToInt(percentPosition.y)];
    }

    private void OnDrawGizmos() {
        // Debug the world size
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x, 1, GridSize.y));

        // Debug the Bottom Start point
        Gizmos.DrawCube(BottomLeftWorldPoint, Vector3.one);

        // Debug Nodes radius
        if (NodesList != null)
            foreach (Node n in NodesList) {
                Gizmos.color = (n.IsWalkable) ? Color.green : Color.red;
                if (n.Equals(PlayerGridPosition)) Gizmos.color = Color.blue;
                if (n.Equals(EndPointGridPosition)) Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(n.WorldPosition - (Vector3.up / 2), new Vector3(NodeDiamter - 0.1f, 1, NodeDiamter - 0.1f));
            }
    }
}
