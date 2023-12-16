using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace AYellowpaper.SerializedCollections {
[CreateAssetMenu(fileName ="TS_", menuName = "Misc/Tileset")]
public class EnvironmentTilesetData : ScriptableObject
{
    public TileBase floorTile;

    [Header("Top")]
    public TileBase wallTop;
    public TileBase wallTopLeft;
    public TileBase wallTopRight;
    public TileBase wallTopLeftAndRight;
    public TileBase wallTopLeftTurnUp;
    public TileBase wallTopRightTurnUp;
    public TileBase wallTopOuterLeft;
    public TileBase wallTopOuterRight;

    [Header("Left")]
    public TileBase wallLeft;
    public TileBase wallSideLeftTRCorner;
    public TileBase wallLeftRightTurn;

    [Header("Right")]
    public TileBase wallRight;
    public TileBase wallSideRightTLCorner;
    public TileBase wallRightLeftTurn;

    [Header("Bottom")]
    public TileBase wallBottom;
    public TileBase wallBottomLeft;
    public TileBase wallBottomRight;
    public TileBase wallBottomLeftAndRight;

    [Header("Full")]
    public TileBase wallFull;
    public TileBase wallFullTop;
    public TileBase wallFullBottom;

    [Header("Inner-Down Left")]
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownLeftTIntersection;

    [Header("Inner-Down Right")]
    public TileBase wallInnerCornerDownRight;
    public TileBase wallInnerCornerDownRightTIntersection;

    [Header(">>Prefabs to Spawn<<")]
    [SerializedDictionary("Key", "GameObject")]
    public SerializedDictionary<string, GameObject> decorations;
    [SerializedDictionary("Key", "GameObject")]
    public SerializedDictionary<string, GameObject> enemies;
}
}