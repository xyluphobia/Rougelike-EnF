using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IlluminateDoor : MonoBehaviour
{
    private GameObject doorToIlluminate;

    private void Awake()
    {
        doorToIlluminate = GameObject.FindGameObjectWithTag("LockedDoor");
    }

    private void OnDestroy()
    {
        doorToIlluminate.GetComponent<Light2D>().enabled = true;
    }
}
