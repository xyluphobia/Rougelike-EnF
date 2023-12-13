using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // how big we want the board to be
    private int columns = 12;
    private int rows = 8;

    // How many potions we want in each level, first is minimum per level, second is maximum per level
    private Count potionCount = new Count(1, 3);
    private Count floorDecorationsCount = new Count(20, 40);
    private Count collectiblesCount = new Count(1, 3);

    private const string TYPE_POTION = "Potion";
    private const string TYPE_FLOOR_DECO = "FloorDecoration";
    private const string TYPE_ENEMY = "Enemy";
    private const string TYPE_COLLECTIBLES = "Collectibles";

    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] potionTiles;
    public GameObject[] enemyTiles;
    public GameObject[] floorDecorations;
    public GameObject[] collectiblesTiles;

    public GameObject wallTop;
    public GameObject wallTopLeft;
    public GameObject wallTopRight;
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallBottom;
    public GameObject wallBottomLeft;
    public GameObject wallBottomRight;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        // starting at 1 instead of 0 and ending at columns/rows - 1 instead of columns/rows leaves a boarder of floor tiles around the edges of the room that remain regular floor tiles
        for (int x = 0; x < columns; x++)     // UPDATED! Starting at 3 and 3 creates a 3x3 protected square in the bottom left.
        {
            for (int y = 0; y < rows; y++)
            {
                if (x == columns - 1 && y == 0) continue;
                if (x == 0 && y == rows - 1) continue;
                if (x == columns - 1 && y == rows - 1) continue;

                if (x <= 3 && y >= 3)
                    gridPositions.Add(new Vector3(x, y, 0f));
                else if (y <= 3 && x >= 3)
                    gridPositions.Add(new Vector3(x, y, 0f));
                else if (x > 3 && y > 3)
                    gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // starting at -1 and ending at +1 columns/rows creates a boarder leaving 0 and equal as a dead zone not interacted with by either loop
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1)
                {
                    if (y == -1)
                    {
                        toInstantiate = wallBottomLeft;
                    }
                    else if (y == rows)
                    {
                        toInstantiate = wallTopLeft;
                    }
                    else
                    {
                        toInstantiate = wallLeft;
                    }
                } 
                else if (x == columns)
                {
                    if (y == -1)
                    {
                        toInstantiate = wallBottomRight;
                    }
                    else if (y == rows)
                    {
                        toInstantiate = wallTopRight;
                    }
                    else
                    {
                        toInstantiate = wallRight;
                    }
                }
                else if (y == -1)
                {
                    toInstantiate = wallBottom;
                }
                else if (y == rows)
                {
                    toInstantiate = wallTop;
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3 (x , y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        // storing a random position from list of available positions then removing that position from list of available
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] inputObjectArray, int minimum, int maximum, string TYPE = null)
    {
        // how many of a given object will be spawned in the level
        int objectCount = Random.Range(minimum, maximum + 1);

        // until number of objects desired added,
        for (int i = 0; i < objectCount; ++i)
        {
            // choose a random position
            Vector3 randomPosition = RandomPosition();

            // choose a random object from the input array of objects
            GameObject tileChoice;
            switch (TYPE)
            {
                case "Potion":
                    tileChoice = SelectPotionBasedOnPercentage();
                    break;

                case "FloorDecoration":
                    tileChoice = SelectFloorDecoBasedOnPercentage();
                    break;

                case "Enemy":
                    tileChoice = SelectEnemyBasedOnPercentage(0.35f, 0.35f);
                    break;

                case "Collectibles":
                    tileChoice = SelectCollectiblesBasedOnPercentage();
                    break;


                default:
                    tileChoice = inputObjectArray[Random.Range(0, inputObjectArray.Length)];
                    break;
            }

            //GameObject tileChoice = inputObjectArray[Random.Range(0, inputObjectArray.Length)];

            // spawn randomly chosen object at randomly chosen position
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    private Vector3 pickExitSpawnPosition() {
        int posX = 0;
        int posY = 0;

        float random = Random.value;

        switch (random)
        {
            case >= 0.5f:
                return new Vector3(0, rows - 1, 0f);
            case < 0.5f:
                posX = columns - 1;
                break;
        }

        random = Random.value;

        switch (random)
        {
            case >= 0.5f:
                posY = 0;
                posX = columns - 1;
                break;
            case < 0.5f:
                posY = rows - 1;
                break;
        }

        return new Vector3(posX, posY, 0f);
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(potionTiles, potionCount.minimum, potionCount.maximum, TYPE_POTION);
        LayoutObjectAtRandom(floorDecorations, floorDecorationsCount.minimum, floorDecorationsCount.maximum, TYPE_FLOOR_DECO);
        LayoutObjectAtRandom(collectiblesTiles, collectiblesCount.minimum, collectiblesCount.maximum, TYPE_COLLECTIBLES);

        // enemyCount is 1E at level 2, 2E at level 4, 3E at level 8 etc
        int enemyCount = (int)Mathf.Log(level, 2f) + 1;
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount, TYPE_ENEMY);
        
        // places the exit in the top right corner inside the outer walls
        Instantiate(exit, pickExitSpawnPosition(), Quaternion.identity);
    }


    private GameObject SelectFloorDecoBasedOnPercentage()
    {
        float randomPercent = Random.value;
        switch (randomPercent) 
        {
            case < 0.1f:
                return floorDecorations[0];  // Spiketrap 10%
            
            case < 0.55f:
                return floorDecorations[1];  // oneBone 45%
            
            case < 0.95f:
                return floorDecorations[2];  // twoBone 40%
            
            case >= 0.95f:
                return floorDecorations[3];  // SkullFloor 5%
        }

        return null;
    }

    private GameObject SelectPotionBasedOnPercentage()
    {
        float randomPercent = Random.value;
        switch (randomPercent) 
        {
            case < 0.5f:
                return potionTiles[0];       // HealthPotion 50%
            case >= 0.5f:
                return potionTiles[1];       // SpeedPotion 50%
        }

        return null;
    }

    private GameObject SelectEnemyBasedOnPercentage(float firstOdds, float secondOdds)
    {
        float randomPercent = Random.value;
        if (randomPercent < firstOdds)  
            return enemyTiles[0];
        else
            return enemyTiles[1];
            
    }

    private GameObject SelectCollectiblesBasedOnPercentage()
    {
        float randomPercent = Random.value;
        switch (randomPercent) 
        {
            case <= 0.01f:
                return collectiblesTiles[0];       // Large Chest 1%
            case <= 0.06f:
                return collectiblesTiles[1];       // Small Chest 5%
            case <= 0.16f:
                return collectiblesTiles[2];       // Gold Coin 10%
            case <= 0.54f:
                return collectiblesTiles[3];       // Silver Coin 38%
            case > 0.54f:
                return collectiblesTiles[4];       // Bronze Coin 46%
        }

        return null;
            
    }
}
