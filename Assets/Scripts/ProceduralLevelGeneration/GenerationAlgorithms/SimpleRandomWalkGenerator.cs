using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkData randomWalkParameters;  // Implements values set in scriptable object to allow for multiple parameters to be saved and swapped between.


    protected override void RunProceduralGeneration() {
        HashSet<Vector2Int> floorPositions  = RunRandomWalk(randomWalkParameters, startPosition);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        // use floor positions to create objects in rooms. try to reference wall types helper to see if were near a wall or not
        tilemapVisualizer.GenerateFloorPositionsBinary(floorPositions);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkData parameters, Vector2Int position) {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions_Run = new HashSet<Vector2Int>();

        for (int i = 0; i < parameters.iterations; i++) {
            var path = PG_Algorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions_Run.UnionWith(path);

            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions_Run.ElementAt(Random.Range(0, floorPositions_Run.Count()));
        }

        return floorPositions_Run;
    }
}
