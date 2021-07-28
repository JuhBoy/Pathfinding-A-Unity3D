using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    // =========================
    //          Costs
    // =========================

    public double FCost => GCost + HCost;
    public double GCost { set; get; }
    public double HCost { set; get; }

    // ====================================
    //                Datas
    // ====================================
    public Node Parent { get; set; }
    public bool IsWalkable { get; set; }
    public Vector3 WorldPosition { get; set; }
    public Vector2Int GridIndexes { get; set; }
    public double Modifier { get; set; }

    public Node(bool mWalkable, Vector3 mWorldPosition, Vector2Int gridIndexes) {
        IsWalkable = mWalkable;
        WorldPosition = mWorldPosition;
        GridIndexes = gridIndexes;
    }

}
