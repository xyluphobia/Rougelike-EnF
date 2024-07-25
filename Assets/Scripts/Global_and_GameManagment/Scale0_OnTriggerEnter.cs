using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale0_OnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.transform.localScale = Vector3.zero;
    }
}
