using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory.instance.addItemToInventory(gameObject.name, gameObject, 1);
        Destroy(gameObject);
    }
}
