using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MOBACharacter : MonoBehaviour
{
    private PlayerController playerController;
    private NavMeshAgent navAgent;

    private GameObject movementIndicatorArrow;
    private Image[] AbilityBarImageHolder;
    private Dictionary<string, Image> AbilityImagesDict = new();

    private bool isTeleporting = false;
    public bool teleportIsOnCooldown = false;
    public float teleportCooldown = 5f;

    private Vector2 clickedPos;
    private Vector2 lastFramePos;
    private Vector2 currentFramePos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        lastFramePos = transform.position;

        AbilityBarImageHolder = GameAssets.i.AbilityBarMOBA.GetComponentsInChildren<Image>(true);
        foreach (Image img in AbilityBarImageHolder)
        {
            AbilityImagesDict.Add(img.transform.name, img);
        }
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
        movementIndicatorArrow = Instantiate(GameAssets.i.movementIndicatorArrow);
        movementIndicatorArrow.transform.localScale = Vector3.zero;

        playerController.SetActiveAbilityBar(GameAssets.i.AbilityBarMOBA);
    }

    private void OnMove_MOBA()
    {
        if (isTeleporting) return;

        clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        navAgent.SetDestination(new Vector3(clickedPos.x, clickedPos.y));

        movementIndicatorArrow.transform.localScale = Vector3.one;
        movementIndicatorArrow.transform.position = navAgent.destination;
    }

    /* Abilities */
    private void OnAbilityOne() /* |Q| Basic Damage Ability */
    {

    }

    private void OnAbilityTwo() /* |W| Interesting Ability ~ Icicle */
    {
        // Target is slowed stacking with each hit until 3 hits where the target is frozen completely
        // Fires towards mouse
    }

    private void OnAbilityThree() /* |E| Movement Ability ~ Teleport */
    {
        if (teleportIsOnCooldown) return;

        StartCoroutine(TeleportAbilityCooldown());
        StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["ETimer"], teleportCooldown));


        isTeleporting = true;
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
        movementIndicatorArrow.transform.localScale = Vector3.zero;

        Vector2 teleportLocaiton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        StartCoroutine(TeleportingAnimationHandler());

        IEnumerator TeleportingAnimationHandler()
        {
            playerController.animator.SetBool("Teleporting", true);
            yield return new WaitForSeconds(0.417f);

            gameObject.transform.position = teleportLocaiton;

            playerController.animator.SetBool("Teleporting", false);
            yield return new WaitForSeconds(0.333f);
            isTeleporting = false;
        }

        IEnumerator TeleportAbilityCooldown()
        {
            teleportIsOnCooldown = true;
            yield return new WaitForSeconds(teleportCooldown);
            teleportIsOnCooldown = false;
        }
    }

    private void OnUltimateAbility() /* |R| Ultimate Ability */
    {

    }

}
