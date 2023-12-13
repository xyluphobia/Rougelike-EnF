using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="TS_", menuName = "Misc/Tileset")]
public class EnvironmentTilesetData : ScriptableObject
{
    public TileBase floorTile;
    public TileBase wallTop;
}
