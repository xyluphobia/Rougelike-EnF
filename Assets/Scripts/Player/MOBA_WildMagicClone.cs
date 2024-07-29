using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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
        GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");

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
        }

        StopCoroutine(activeAI);
        Destroy(gameObject);
    }

    public void OnCloneDied()
    {
        StartCoroutine(TieUpAndDestroy(0f));
    }

    IEnumerator WASDCharacterAI()
    {
        yield return new WaitForSeconds(1);
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
