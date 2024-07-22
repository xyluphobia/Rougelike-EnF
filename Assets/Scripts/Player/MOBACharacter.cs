using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MOBACharacter : MonoBehaviour
{
    private PlayerController playerController;
    private NavMeshAgent navAgent;

    Vector2 clickedPos;
    Vector2 lastFramePos;
    Vector2 currentFramePos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        lastFramePos = transform.position;
    }

    private void FixedUpdate() // could find direction by saving previous step and comparing against current position then checking which direction was travelled
    {
        currentFramePos = transform.position;

        if (currentFramePos != lastFramePos)
        {
            Vector2 diffVector = new Vector2(System.Math.Abs(currentFramePos.x - lastFramePos.x), System.Math.Abs(currentFramePos.y - lastFramePos.y));

            float diffXPercent = diffVector.x / (diffVector.x + diffVector.y);
            float diffYPercent = diffVector.y / (diffVector.x + diffVector.y);

            if (currentFramePos.x < lastFramePos.x)
                diffXPercent *= -1;
            if (currentFramePos.y < lastFramePos.y)
                diffYPercent *= -1;

            playerController.animator.SetFloat("Horizontal", diffXPercent);
            playerController.animator.SetFloat("Vertical", diffYPercent);
            playerController.animator.SetFloat("Speed", diffVector.sqrMagnitude);

            lastFramePos = currentFramePos;
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
        clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*
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
        */
        navAgent.SetDestination(new Vector3(clickedPos.x, clickedPos.y));
    }
}
