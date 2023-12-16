using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList);
        
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighborsBinaryValue = "";
            foreach (var direction in Direction2D.cardinalDirectionsList) {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                    neighborsBinaryValue += "1";
                else
                    neighborsBinaryValue += "0";
            }

            tilemapVisualizer.PaintSingleBasicWall(position, neighborsBinaryValue);
        }
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighborsBinaryValue = "";
            foreach (var direction in Direction2D.eightDirectionsList) {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                    neighborsBinaryValue += "1";
                else
                    neighborsBinaryValue += "0";
            }

            tilemapVisualizer.PaintSingleCornerWall(position, neighborsBinaryValue);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions_Find, List<Vector2Int> directionList) {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions_Find) {
            foreach (var direction in directionList) {

                var neighborPosition = position + direction;
                if (!floorPositions_Find.Contains(neighborPosition))
                    wallPositions.Add(neighborPosition);

            }
        }
        
        return wallPositions;
    }
}
