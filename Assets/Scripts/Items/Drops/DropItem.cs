using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private GameObject itemToDrop;
    [SerializeField] private Vector3 dropLocation;

    private void OnDeath()
    {
        Instantiate(itemToDrop, dropLocation, Quaternion.identity);
    }
}
