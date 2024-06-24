using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParentOnDeath : MonoBehaviour
{
    private void OnDeath()
    {
        Destroy(gameObject, 1.6f);
    }
}
