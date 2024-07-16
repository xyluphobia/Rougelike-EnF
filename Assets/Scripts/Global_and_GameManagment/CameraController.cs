using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;


    void Start()
    {
        SetCameraTarget();
    }
    public void SetCameraTarget(GameObject targetToSet = null)
    {
        if (targetToSet != null)
            target = targetToSet.transform;
        else
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
