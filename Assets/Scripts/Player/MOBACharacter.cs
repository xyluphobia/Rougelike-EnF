using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOBACharacter : MonoBehaviour
{
    private PlayerController playerController;

    Vector2 lastClickedPos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if ((Vector2)transform.position != lastClickedPos)
        {
            float step = playerController.movementSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, lastClickedPos, step);
        }
        else
        {
            playerController.animator.SetFloat("Speed", 0);
        }
    }

    public void TakeControl()
    {

    }

    private void OnMove_MOBA()
    {
        lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float diffX = lastClickedPos.x - transform.position.x;
        float diffY = lastClickedPos.y - transform.position.y;

        playerController.animator.SetFloat("Horizontal", diffX);
        playerController.animator.SetFloat("Vertical", diffY);
        playerController.animator.SetFloat("Speed", lastClickedPos.sqrMagnitude);


        if (diffX > diffY)
        {
            diffY = (diffY / diffX) * 100;
            diffX = 1;
            playerController.animator.SetFloat("lastHorizontal", diffX);
        }
        else
        {
            playerController.animator.SetFloat("lastVertical", diffY);
        }
    }
}
