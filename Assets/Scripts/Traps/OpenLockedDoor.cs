using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockedDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] doorsToOpen;
    [SerializeField] private GameObject doorKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Inventory.instance.haveItem(doorKey.name))
        {
            Debug.Log("got here");
            foreach (GameObject door in doorsToOpen)
            {
                door.GetComponent<Animator>().SetBool("doorOpen", true);
                door.GetComponent<BoxCollider2D>().enabled = false;
            }
            Inventory.instance.removeItemFromInventory(doorKey.name, 1);
        }

        Debug.Log("ObjectName: " + doorKey.name);
    }
}
