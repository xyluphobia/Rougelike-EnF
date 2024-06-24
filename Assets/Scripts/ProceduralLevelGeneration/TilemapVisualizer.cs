using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private EnvironmentTilesetData envTS_data;

    private bool playerSpawned = false;
    private bool exitSpawned = false;
    private bool bossSpawned = false;

    private Transform enemyholder;
    private Transform propHolder;
    private Transform itemHolder;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) {
        floorTilemap.ClearAllTiles(); // Clear all current floor tiles. Remove this for it to iterate on the original instead of replace.
        wallTilemap.ClearAllTiles();
        
        PaintTiles(floorPositions, floorTilemap, envTS_data.floorTile);
    }


    public void GenerateFloorPositionsBinary(HashSet<Vector2Int> floorPositions)
    /* 
        Makes a matrix of 8 positions in a clockwise direction around the each position in the floorPositions list.
    */
    {
        HashSet<Vector2Int> usedPositions = new();
        float iterations = 0f;
        float floorPositionsCount = floorPositions.Count;

        enemyholder = Instantiate(GameAssets.i.EnemyHolder).transform;
        propHolder = Instantiate(GameAssets.i.PropHolder).transform;
        itemHolder = Instantiate(GameAssets.i.ItemHolder).transform;

        foreach (var position in floorPositions)
        {
            string neighborsBinaryValue = "";
            foreach (var direction in Direction2D.eightDirectionsList) {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                    neighborsBinaryValue += "1";
                else
                    neighborsBinaryValue += "0";
            }

            if (!playerSpawned && iterations >= floorPositionsCount * 0.1f) {
                Instantiate(GameAssets.i.player, (Vector3Int)position, Quaternion.identity);
                playerSpawned = true;
                usedPositions.Add(position);
            }
            else if (!exitSpawned && iterations >= floorPositionsCount * 0.9f) {
                exitSpawned = true;
                Instantiate(GameAssets.i.exit, (Vector3Int)position, Quaternion.identity);
                usedPositions.Add(position);
            }
            else {
                float randomValue = Random.value;
                if (randomValue < 0.85f) {
                    int typeAsInt = Convert.ToInt32(neighborsBinaryValue, 2);
                    usedPositions = GenerateProps(position, typeAsInt, usedPositions);
                }
                else if (randomValue < 0.98) {
                    usedPositions = GenerateCollectables(position, usedPositions);
                }
                else
                    usedPositions = GeneratePotions(position, usedPositions);
            }

            
            iterations += 1f;
        }
        
        foreach (var position in floorPositions)
        {
            if (!usedPositions.Contains(position)) {
                string neighborsBinaryValue = "";
                foreach (var direction in Direction2D.eightDirectionsList) {
                    var neighborPosition = position + direction;
                    if (floorPositions.Contains(neighborPosition))
                        neighborsBinaryValue += "1";
                    else
                        neighborsBinaryValue += "0";
                }

                int typeAsInt = Convert.ToInt32(neighborsBinaryValue, 2);
                usedPositions = GenerateEnemies(position, typeAsInt, usedPositions);
            }
        }
        
    }

    private HashSet<Vector2Int> GenerateProps(Vector2Int position, int typeAsInt, HashSet<Vector2Int> usedPositions)   
    // REMOVE POSITIONS PROPS ARE PLACED ON FROM FLOOR POSITIONS LIST AS RETURN
    {/*
        if (typeAsInt > 10 && !playerSpawned) {
            Instantiate(GameAssets.i.player, (Vector3Int)position, Quaternion.identity);
            playerSpawned = true;
        }*/

        if (typeAsInt.ToString().Contains("1")) {
            PickAndInstantiateProp(position);
            usedPositions.Add(position);
        }
        else if (Random.value > 0.9f) {
            PickAndInstantiateProp(position);
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    private void PickAndInstantiateProp(Vector2Int position) {
        float ranVal = Random.value;

        if (ranVal < 0.1)
            Instantiate(envTS_data.decorations["skull"], (Vector3Int)position, Quaternion.identity, propHolder);
        else if (ranVal < 0.3)
            Instantiate(envTS_data.decorations["oneBone"], (Vector3Int)position, Quaternion.identity, propHolder);
        else if (ranVal < 0.7)
            Instantiate(envTS_data.decorations["twoBone"], (Vector3Int)position, Quaternion.identity, propHolder);
    }

    private HashSet<Vector2Int> GenerateCollectables(Vector2Int position, HashSet<Vector2Int> usedPositions)   
    {
        if (Random.value < 0.2f) {
            PickAndInstantiateCollectable(position);
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    private void PickAndInstantiateCollectable(Vector2Int position) {
        float ranVal = Random.value;

        if (ranVal < 0.4)
            Instantiate(envTS_data.collectibles["bronzeCoin"], (Vector3Int)position, Quaternion.identity, itemHolder);
        else if (ranVal < 0.65)
            Instantiate(envTS_data.collectibles["silverCoin"], (Vector3Int)position, Quaternion.identity, itemHolder);
        else if (ranVal < 0.75)
            Instantiate(envTS_data.collectibles["goldCoin"], (Vector3Int)position, Quaternion.identity, itemHolder);
        else if (ranVal < 0.80)
            Instantiate(envTS_data.collectibles["woodenChest"], (Vector3Int)position, Quaternion.identity, itemHolder);
        else if (ranVal > 0.98)
            Instantiate(envTS_data.collectibles["royalChest"], (Vector3Int)position, Quaternion.identity, itemHolder);
    }

    private HashSet<Vector2Int> GeneratePotions(Vector2Int position, HashSet<Vector2Int> usedPositions)   
    {
        if (Random.value > 0.6f) {
            PickAndInstantiatePotion(position);
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    private void PickAndInstantiatePotion(Vector2Int position) {
        float ranVal = Random.value;

        if (ranVal < 0.55)
            Instantiate(envTS_data.potions["health"], (Vector3Int)position, Quaternion.identity, itemHolder);
        else
            Instantiate(envTS_data.potions["speed"], (Vector3Int)position, Quaternion.identity, itemHolder);
    }

    private HashSet<Vector2Int> GenerateEnemies(Vector2Int position, int typeAsInt, HashSet<Vector2Int> usedPositions)    {
        if (Random.value > 0.97f) {
            PickAndInstantiateEnemy(position);
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    private void PickAndInstantiateEnemy(Vector2Int position) {
        if(!GameManager.instance.isBossLevel)
        {
            float ranVal = Random.value;

            if (ranVal < 0.65)
                Instantiate(envTS_data.enemies["slime"], (Vector3Int)position, Quaternion.identity, enemyholder);
            else if (ranVal < 1.0)
                Instantiate(envTS_data.enemies["serpent"], (Vector3Int)position, Quaternion.identity, enemyholder);
        }

        else if (!bossSpawned)
        {
            bossSpawned = true;
            Instantiate(envTS_data.enemies["rotator"], (Vector3Int)position, Quaternion.identity, enemyholder);
        }
    }

    public void PaintSingleBasicWall(Vector2Int position, string binaryType) 
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
            tile = envTS_data.wallTop;
        else if (WallTypesHelper.wallTopLeftTurnUp.Contains(typeAsInt))
            tile = envTS_data.wallTopLeftTurnUp;
        else if (WallTypesHelper.wallTopRightTurnUp.Contains(typeAsInt))
            tile = envTS_data.wallTopRightTurnUp;
        else if (WallTypesHelper.wallTopOuterLeft.Contains(typeAsInt))
            tile = envTS_data.wallTopOuterLeft;
        else if (WallTypesHelper.wallTopOuterRight.Contains(typeAsInt))
            tile = envTS_data.wallTopOuterRight;
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
            tile = envTS_data.wallRight;
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
            tile = envTS_data.wallLeft;
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
            tile = envTS_data.wallBottom;
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
            tile = envTS_data.wallFull;
        else if (WallTypesHelper.wallFullTop.Contains(typeAsInt)) 
            tile = envTS_data.wallFullTop;
        else if (WallTypesHelper.wallFullBottom.Contains(typeAsInt)) 
            tile = envTS_data.wallFullBottom;


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
        else if (WallTypesHelper.wallBottomLeftAndRight.Contains(typeAsInt))
            tile = envTS_data.wallBottomLeftAndRight;
        else if (WallTypesHelper.wallSideRightTLCorner.Contains(typeAsInt))
            tile = envTS_data.wallSideRightTLCorner;
        else if (WallTypesHelper.wallSideLeftTRCorner.Contains(typeAsInt))
            tile = envTS_data.wallSideLeftTRCorner;
        else if (WallTypesHelper.wallTopLeftAndRight.Contains(typeAsInt))
            tile = envTS_data.wallTopLeftAndRight;
        else if (WallTypesHelper.wallLeftRightTurn.Contains(typeAsInt))
            tile = envTS_data.wallLeftRightTurn;
        else if (WallTypesHelper.wallRightLeftTurn.Contains(typeAsInt))
            tile = envTS_data.wallRightLeftTurn;
        else if (WallTypesHelper.wallInnerCornerDownLeftTIntersection.Contains(typeAsInt))
            tile = envTS_data.wallInnerCornerDownLeftTIntersection;
        else if (WallTypesHelper.wallInnerCornerDownRightTIntersection.Contains(typeAsInt))
            tile = envTS_data.wallInnerCornerDownRightTIntersection;


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
