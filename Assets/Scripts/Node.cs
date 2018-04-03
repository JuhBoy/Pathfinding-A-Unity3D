using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    // =========================
    //          Costs
    // =========================
    private double? _gCost;
    private double? _hCost;

    public double? FCost {
        get { return _gCost + _hCost; } 
    }
    public double? GCost { set { _gCost = value; } get { return _gCost; } }
    public double HCost { set { _hCost = value; } get { return (double) _hCost; } }

    // ====================================
    //                Datas
    // ====================================
    public Node parent;
    public bool IsWalkable { get; set; }
    public Vector3 WorldPosition { get; set; }
    public Vector2Int GridIndexes { get; set; }
    public double Modifier { get; set; }

    public Node(bool m_walkable, Vector3 m_worldPosition, Vector2Int gridIndexes) {
        IsWalkable = m_walkable;
        WorldPosition = m_worldPosition;
        GridIndexes = gridIndexes;
    }

}
