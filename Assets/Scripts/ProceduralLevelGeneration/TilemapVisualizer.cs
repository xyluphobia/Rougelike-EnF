using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private EnvironmentTilesetData envTS_data;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) {
        floorTilemap.ClearAllTiles(); // Clear all current floor tiles. Remove this for it to iterate on the original instead of replace.
        wallTilemap.ClearAllTiles();
        
        PaintTiles(floorPositions, floorTilemap, envTS_data.floorTile);
    }

    public void PaintSingleBasicWall(Vector2Int position, string binaryType) 
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
            tile = envTS_data.wallTop;
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
            tile = envTS_data.wallRight;
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
            tile = envTS_data.wallLeft;
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
            tile = envTS_data.wallBottom;
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
            tile = envTS_data.wallFull;



        if (tile)
            PaintSingleTile(wallTilemap, tile, position);
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        
        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
            tile = envTS_data.wallInnerCornerDownLeft;
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
            tile = envTS_data.wallInnerCornerDownRight;
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
            tile = envTS_data.wallBottomLeft;
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
            tile = envTS_data.wallBottomRight;
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
            tile = envTS_data.wallTopLeft;
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
            tile = envTS_data.wallTopRight;
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
            tile = envTS_data.wallFull;
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
            tile = envTS_data.wallBottom;

        if (tile)
            PaintSingleTile(wallTilemap, tile, position);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) {
        foreach (var position in positions) {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
}
