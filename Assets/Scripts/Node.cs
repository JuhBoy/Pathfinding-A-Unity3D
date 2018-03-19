using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    // =========================
    //          Costs
    // =========================
    private int _gCost;
    private int _hCost;

    public int FCost { get { return _gCost + _hCost; } }
    public int GCost { set { _gCost = value; } }
    public int HCost { set { _hCost = value; } }

    // ====================================
    //                Datas
    // ====================================
    public bool IsWalkable { get; set; }
    public Vector3 WorldPosition { get; set; }
    public Vector2Int GridIndexes { get; set; }

    public Node(bool m_walkable, Vector3 m_worldPosition, Vector2Int gridIndexes) {
        IsWalkable = m_walkable;
        WorldPosition = m_worldPosition;
        GridIndexes = gridIndexes;
    }

}
