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

    public void PaintSingleBasicWall(Vector2Int position) {
        PaintSingleTile(wallTilemap, envTS_data.wallTop, position);
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
