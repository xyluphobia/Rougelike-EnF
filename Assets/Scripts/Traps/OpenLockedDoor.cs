using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class OpenLockedDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] doorsToOpen;
    [SerializeField] private GameObject doorKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Inventory.instance.haveItem(doorKey.name))
        {
            foreach (GameObject door in doorsToOpen)
            {
                door.GetComponent<Animator>().SetBool("doorOpen", true);
                door.GetComponent<BoxCollider2D>().enabled = false;
                door.GetComponent<NavMeshObstacle>().enabled = false;

                if (door.TryGetComponent(out Light2D lightScript))
                {
                    lightScript.enabled = false;
                }
            }
            Inventory.instance.removeItemFromInventory(doorKey.name, 1);
        }
    }
}
