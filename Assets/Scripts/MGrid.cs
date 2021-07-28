using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public Heuristic heuristic;
    private PathFinder _pathFinder;
    private Stack<Node> _orderedNodes;

    // =====================
    //         Move
    // =====================
    private Node _currentDestination;

    void Start() {
        _pathFinder = new PathFinder(this, heuristic);
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
                Debug.Log("e:" + e);
            }
        }

        if (_orderedNodes != null)
            MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && _orderedNodes == null) {
            _pathFinder.ClearCache();
            _orderedNodes = _pathFinder.Compute(PlayerGridPosition, EndPointGridPosition);
        }
        
        if (Input.GetKeyDown(KeyCode.Return)) {
            _orderedNodes = null;
            _pathFinder.ClearCache();
        }
    }

    #region Internal

    private void FillNodes() {
        NodesList = new Node[Rows, Cols];
        BottomLeftWorldPoint = transform.position - (Vector3.forward * GridSize.y / 2) - (Vector3.right * GridSize.x / 2) +
                               ((Vector3.right + Vector3.forward) * NodeRadius);

        for (int r = 0; r < Rows; r++) {
            for (int c = 0; c < Cols; c++) {
                var worldPosition = new Vector3(
                    x: BottomLeftWorldPoint.x + (r * NodeDiamter),
                    y: 0,
                    z: BottomLeftWorldPoint.z + (c * NodeDiamter)
                );
                bool walkable = !Physics.CheckSphere(worldPosition, NodeRadius, UnwalkableMask);
                NodesList[r, c] = new Node(walkable, worldPosition, new Vector2Int(r, c));

                // TODO: High cost zone, to remove
                if (r <= 3 && c <= 9) {
                    NodesList[r, c].Modifier = 2f;
                }
            }
        }
    }

    private Node NodePositionFromWorldPosition(Vector3 worldPosition) {
        var percentPosition = new Vector2 {
            x = ((worldPosition.x + (GridSize.x / 2 - 1)) - transform.position.x) / GridSize.x * Rows,
            y = ((worldPosition.z + (GridSize.y / 2 - 1)) - transform.position.z) / GridSize.y * Cols
        };
        return NodesList[Mathf.RoundToInt(percentPosition.x), Mathf.RoundToInt(percentPosition.y)];
    }

    #endregion

    #region Public Helpers

    public Node[] GetNeighbours(Node node) {
        int row = node.GridIndexes.x;
        int col = node.GridIndexes.y;

        Vector2Int[] vector2Ints = new Vector2Int[] {
            new Vector2Int(row, col - 1), // left
            new Vector2Int(row - 1, col), // bottom
            new Vector2Int(row, col + 1), // right
            new Vector2Int(row + 1, col), // top 
            new Vector2Int(row + 1, col - 1), // topLeft
            new Vector2Int(row + 1, col + 1), // topRight
            new Vector2Int(row - 1, col - 1), // bottomLeft
            new Vector2Int(row - 1, col + 1) // bottomRight
        };
        List<Node> nodes = new List<Node>();

        foreach (Vector2Int v in vector2Ints) {
            if (v.x >= 0 && v.x < Rows &&
                v.y >= 0 && v.y < Cols) {
                nodes.Add(NodesList[v.x, v.y]);
            }
        }

        return nodes.ToArray();
    }

    #endregion

    private void MovePlayer() {
        if (_orderedNodes.Any() &&
            (_currentDestination == null || DistanceBetweenIsUnder2DEpsilon(PlayerWorldPosition.position, _currentDestination.WorldPosition))) {
            _currentDestination = _orderedNodes.Pop();
        }

        if (_currentDestination != null) {
            var target = new Vector3(_currentDestination.WorldPosition.x, PlayerWorldPosition.position.y, _currentDestination.WorldPosition.z);
            PlayerWorldPosition.position = Vector3.MoveTowards(PlayerWorldPosition.position, target, 9 * Time.deltaTime);

            if (!_orderedNodes.Any() && DistanceBetweenIsUnder2DEpsilon(PlayerWorldPosition.position, _currentDestination.WorldPosition)) {
                _currentDestination = null;
                _orderedNodes.Clear();
                _orderedNodes = null;
            }
        }
    }

    private static bool DistanceBetweenIsUnder2DEpsilon(Vector3 a, Vector3 b) {
        return Vector2.Distance(
            new Vector2(a.x, a.z),
            new Vector2(b.x, b.z)) <= float.Epsilon;
    }

#if UNITY_EDITOR
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

                Gizmos.DrawWireCube(n.WorldPosition - (Vector3.up / 2), new Vector3(NodeDiamter - 0.1f, 0.8f, NodeDiamter - 0.1f));
            }

        if (_orderedNodes != null)
            foreach (Node n in _orderedNodes) {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(n.WorldPosition, new Vector3(NodeRadius, 0.4f, NodeRadius));
            }
    }
#endif
}