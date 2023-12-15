using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="TS_", menuName = "Misc/Tileset")]
public class EnvironmentTilesetData : ScriptableObject
{
    public TileBase floorTile;

    public TileBase wallTop;
    public TileBase wallTopLeft;
    public TileBase wallTopRight;
    public TileBase wallTopOuterLeft;
    public TileBase wallTopOuterRight;
    public TileBase wallTopFull;

    public TileBase wallLeft;
    public TileBase wallRight;
    public TileBase wallSideRightTLCorner;
    public TileBase wallSideLeftTRCorner;

    public TileBase wallBottom;
    public TileBase wallBottomLeft;
    public TileBase wallBottomRight;
    public TileBase wallBottomFull;

    public TileBase wallFull;
    public TileBase wallFullTop;
    public TileBase wallFullBottom;

    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownRight;
}
