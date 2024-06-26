using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /* This script is held on the GameManager not the player as it needs to persist */
    public class Item
    {
        public GameObject itemObject;
        public int itemCount;
    }

    public static Inventory instance = null;
    private Dictionary<string, Item> heldItems = new();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void addItemToInventory(string key, GameObject item, int amount)
    {
        // check if item is in collection
        // if item is in collection increase item count by provided int
        // if item is not in collection add it with the count provided

        if(key.Contains("(Clone)"))
        {
            key = key.Remove(key.Length - 7);
        }

        if (heldItems.ContainsKey(key))
        {
            heldItems[key].itemCount += amount;
        }
        else
        {
            heldItems.Add(key, new Item { itemObject = item, itemCount = amount });
        }
        Debug.Log(key);
    }

    public void removeItemFromInventory(string key, int amount)
    {
        // find item by key
        // decrease item count by amount
        // if item amount is 0 remove it from dictionary
        if (heldItems.ContainsKey(key))
        {
            heldItems[key].itemCount -= amount;
            if (heldItems[key].itemCount <= 0)
                heldItems.Remove(key);
        }
    }

    public bool haveItem(string key)
    {
        if (heldItems.ContainsKey(key))
            return true;
        else
            return false;
    }
}
