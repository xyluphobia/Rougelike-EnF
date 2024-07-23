using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    private Vector3 targetPoint;

    private readonly float lookAheadSpeed = 5f;
    private readonly float lookAheadDistanceX = 2.5f;
    private readonly float lookAheadDistanceY = 1.5f;
    private Vector2 lookOffset;

    private Vector2 targetCurrentPosition;
    private Vector2 targetLastPosition;

    void Start()
    {
        SetCameraTarget();
        targetPoint.z = transform.position.z;
        targetLastPosition = target.transform.position;
    }
    public void SetCameraTarget(GameObject targetToSet = null)
    {
        if (targetToSet != null)
            target = targetToSet;
        else
            target = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        //transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        targetCurrentPosition = target.transform.position;

        if (targetCurrentPosition.x > targetLastPosition.x)
            lookOffset.x = Mathf.Lerp(lookOffset.x, lookAheadDistanceX, lookAheadSpeed * Time.deltaTime);
        else if (targetCurrentPosition.x < targetLastPosition.x)
            lookOffset.x = Mathf.Lerp(lookOffset.x, -lookAheadDistanceX, lookAheadSpeed * Time.deltaTime);
        else
            lookOffset.x = Mathf.Lerp(lookOffset.x, 0, 4f * Time.deltaTime);

        if (targetCurrentPosition.y > targetLastPosition.y)
            lookOffset.y = Mathf.Lerp(lookOffset.y, lookAheadDistanceY, lookAheadSpeed * Time.deltaTime);
        else if (targetCurrentPosition.y < targetLastPosition.y)
            lookOffset.y = Mathf.Lerp(lookOffset.y, -lookAheadDistanceY, lookAheadSpeed * Time.deltaTime);
        else
            lookOffset.y = Mathf.Lerp(lookOffset.y, 0, 4f * Time.deltaTime);

        targetPoint.x = targetCurrentPosition.x + lookOffset.x;
        targetPoint.y = targetCurrentPosition.y + lookOffset.y;


        transform.position = Vector3.Lerp(transform.position, targetPoint, 4f * Time.deltaTime);
        targetLastPosition = target.transform.position;
    }
}
