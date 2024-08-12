using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class MOBA_WildMagicClone : MonoBehaviour
{
    private Coroutine activeAI;
    private Coroutine destroyRoutine;
    private string cloneType;

    void Start()
    {
        destroyRoutine = StartCoroutine(TieUpAndDestroy(30f));
        transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.tag = "Untagged";

        if (gameObject.name.Equals(GameAssets.i.WASDCharacter.name) || gameObject.name.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
        {
            activeAI = StartCoroutine(WASDCharacterAI());
            cloneType = "WASD";
        }
        else
        {
            activeAI = StartCoroutine(MOBACharacterAI());
            cloneType = "MOBA";
        }
    }

    IEnumerator TieUpAndDestroy(float timeLeft)
    {
        if (destroyRoutine != null)
            StopCoroutine(destroyRoutine);

        yield return new WaitForSeconds(timeLeft);
        SoundManager.instance.PlaySound(GameAssets.i.ultimateCloneDespawnSfx);
        GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
        gameObject.transform.localScale = Vector3.zero;

        if (playerCharacter.TryGetComponent<MOBACharacter>(out MOBACharacter script))
        {
            script.currentClone = null;
            script.StartRTimerCoroutine();
        }
        else
        {
            playerCharacter.GetComponent<PlayerController>().setInactivePlayer();

            GameObject newPlayer = Instantiate(GameAssets.i.MOBACharacter, playerCharacter.transform.position, Quaternion.identity);
            newPlayer.GetComponent<PlayerController>().setActivePlayer();


            Destroy(playerCharacter);
            playerCharacter = newPlayer;
        }
        playerCharacter.GetComponent<MOBACharacter>().disableMovement = true;
        playerCharacter.GetComponent<MOBACharacter>().canCast = false;
        playerCharacter.GetComponent<NavMeshAgent>().ResetPath();

        StartCoroutine(SwapBackAnimation());
        StopCoroutine(activeAI);

        IEnumerator SwapBackAnimation()
        {
            playerCharacter.GetComponent<PlayerController>().animator.SetTrigger("SwapBackFromUlt");
            //SoundManager.instance.PlaySound();   ~    Ult end sound effect here
            yield return new WaitForSeconds(0.333f);
            playerCharacter.GetComponent<MOBACharacter>().disableMovement = false;
            playerCharacter.GetComponent<MOBACharacter>().canCast = true;
            Destroy(gameObject);
        }
    }

    public void OnCloneDied()
    {
        StartCoroutine(TieUpAndDestroy(0f));
    }

    IEnumerator WASDCharacterAI()
    {
        WASDCharacter WASDScript = GetComponent<WASDCharacter>();
        PlayerController playerController = GetComponent<PlayerController>();
        /*GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = false;*/
        //Physics2D.IgnoreLayerCollision(6, 9, true);

        gameObject.AddComponent<NavMeshAgent>();
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navAgent.avoidancePriority = 99;

        GameObject targetEnemy = Tools.instance.FindClosestObjectByTag(transform.position);
        bool inRange = false;
        navAgent.SetDestination(targetEnemy.transform.position);
        bool meleeIsOnCooldown = false;

        while (true) 
        { 
            while (!inRange)
            {
                if (Vector2.Distance(transform.position, GetTargetOrReaquire().transform.position) < 2.8f)
                    inRange = true;

                Vector2 diffVector = new Vector2(GetTargetOrReaquire().transform.position.x - transform.position.x, GetTargetOrReaquire().transform.position.y - transform.position.y);
                float diffXPercent = diffVector.x / (diffVector.x + diffVector.y);
                float diffYPercent = diffVector.y / (diffVector.x + diffVector.y);

                if (GetTargetOrReaquire().transform.position.x < transform.position.x)
                    diffXPercent *= -1;
                if (GetTargetOrReaquire().transform.position.y < transform.position.y)
                    diffYPercent *= -1;

                playerController.animator.SetFloat("Horizontal", diffXPercent);
                playerController.animator.SetFloat("Vertical", diffYPercent);
                playerController.animator.SetFloat("Speed", 1f);

                if (diffXPercent > 0.7f || diffXPercent < -0.7f || diffYPercent > 0.7f || diffYPercent < -0.7f)
                {
                    playerController.animator.SetFloat("lastHorizontal", diffXPercent);
                    playerController.animator.SetFloat("lastVertical", diffYPercent);
                }

                navAgent.SetDestination(GetTargetOrReaquire().transform.position);
                yield return null;
            }

            if (inRange)
            {
                navAgent.ResetPath();
                playerController.animator.SetFloat("Speed", 0);
                if (!meleeIsOnCooldown)
                {
                    WASDScript.Melee(true);
                    StartCoroutine(MeleeAbilityCooldown());
                }
            }
            else if (targetEnemy == null)
            {
                navAgent.ResetPath();
                playerController.animator.SetFloat("Speed", 0);
            }

            inRange = false;
            yield return new WaitForSeconds(0.5f);
        }


        GameObject GetTargetOrReaquire()
        {
            if (targetEnemy != null)
                return targetEnemy;
            else
                return Tools.instance.FindClosestObjectByTag(transform.position);
        }

        IEnumerator MeleeAbilityCooldown()
        {
            meleeIsOnCooldown = true;
            yield return new WaitForSeconds(1.5f);
            meleeIsOnCooldown = false;
        }
    }

    IEnumerator MOBACharacterAI()
    {
        MOBACharacter MOBAScript = GetComponent<MOBACharacter>();
        MOBAScript.firedByAi = true;
        yield return new WaitForSeconds(1f);

        while (true)
        {
            MOBAScript.OnAbilityOne();
            yield return new WaitForSeconds(3.5f);

            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(3.5f);

            MOBAScript.OnAbilityThree();
            yield return new WaitForSeconds(3.5f);
        }
    }

    public void SwapWithPlayer(Vector3 newPosition)
    {
        GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");

        switch (cloneType)
        {
            case "WASD":
                playerCharacter.GetComponent<PlayerController>().setInactivePlayer();
                Destroy(playerCharacter);

                GameObject newPlayer = Instantiate(GameAssets.i.WASDCharacter, transform.position, Quaternion.identity);
                newPlayer.GetComponent<PlayerController>().setActivePlayer();

                SpawnCloneAtPlayerPosition(newPosition);
                StopAllCoroutines();
                Destroy(gameObject);
                break;

            case "MOBA":
                playerCharacter.GetComponent<NavMeshAgent>().Warp(transform.position);
                GetComponent<NavMeshAgent>().Warp(newPosition);
                break;
        }

    }

    void SpawnCloneAtPlayerPosition(Vector3 playerPosition)
    {
        if (NavMesh.SamplePosition(playerPosition, out NavMeshHit closestNavPosition, 100, -1))
        {
            GameObject character = Instantiate(GameAssets.i.MOBACharacter, closestNavPosition.position, Quaternion.identity);

            character.GetComponent<Rigidbody2D>().isKinematic = true;
            character.GetComponent<Rigidbody2D>().simulated = false;
            character.GetComponent<SpriteRenderer>().color = new Color32(47, 255, 255, 137);
            character.GetComponentInChildren<Light2D>().color = new Color32(143, 236, 255, 255);
            if (character.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 99;
            }
            character.AddComponent<MOBA_WildMagicClone>();
        }
        else
        {
            Debug.Log("Clone: No closest nav position.");
        }
    }
}
