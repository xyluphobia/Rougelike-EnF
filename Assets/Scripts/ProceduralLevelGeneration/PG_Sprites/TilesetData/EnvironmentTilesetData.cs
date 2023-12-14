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
    public TileBase wallLeft;
    public TileBase wallRight;
    public TileBase wallBottom;
    public TileBase wallBottomLeft;
    public TileBase wallBottomRight;

    public TileBase wallFull;
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownRight;
}
