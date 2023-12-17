using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Apple;
using Random = UnityEngine.Random;

public class RoomFirstGenerator : SimpleRandomWalkGenerator
{
    [SerializeField] private int minRoomWidth = 10, minRoomHeight = 10;
    [SerializeField] private int dungeonWidth = 70, dungeonHeight = 70;
    [SerializeField] [Range(0,10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    public override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms() {
        var roomsList = PG_Algorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, 
          new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        
        HashSet<Vector2Int> floor = new();

        if (randomWalkRooms) {
            floor = CreateRoomsRandomWalk(roomsList);
        }
        else {
            floor = CreateSimpleRooms(roomsList);
        }
        
        List<Vector2Int> roomCenters = new();
        foreach (var room in roomsList) {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        tilemapVisualizer.GenerateFloorPositionsBinary(floor);
    }

    private HashSet<Vector2Int> CreateRoomsRandomWalk(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();
        for (int i = 0; i < roomsList.Count; i++) {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor) {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset)) {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList) {
        HashSet<Vector2Int> floor = new();

        foreach (var room in roomsList) {    //  https://youtu.be/pWZg1oChtnc?si=gEjZS_WjGDRAToGR&t=603
            for (int col = offset; col < room.size.x - offset; col++) {
                for (int row = offset; row < room.size.y - offset; row++) {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters) {
        HashSet<Vector2Int> corridors = new();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0) {
            Vector2Int closest = findClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            
            corridors.UnionWith(newCorridor);
        }
        

        return corridors;
    }

    private Vector2Int findClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters) {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var roomCenter in roomCenters) {
            float currentDistance = Vector2Int.Distance(roomCenter, currentRoomCenter);
            if (currentDistance < distance) {
                closest = roomCenter;
                distance = currentDistance;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination) {
        HashSet<Vector2Int> corridor = new();
        var position = currentRoomCenter;
        corridor.Add(position);

        //int corridorLength = 0;

        while (position.y != destination.y) {
            if (destination.y > position.y) 
                position += Vector2Int.up;
            else if (destination.y < position.y) 
                position += Vector2Int.down;
            
            corridor.Add(position);

            //corridorLength++;
        }

        while (position.x != destination.x) {
            if (destination.x > position.x) 
                position += Vector2Int.right;
            else if (destination.x < position.x)
                position += Vector2Int.left;
            
            corridor.Add(position);

            //corridorLength++;
        }

        //if (corridorLength > 20)
        //    return new();

        return corridor;
    }
}
