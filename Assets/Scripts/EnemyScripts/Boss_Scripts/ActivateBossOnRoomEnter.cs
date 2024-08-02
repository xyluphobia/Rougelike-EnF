using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ActivateBossOnRoomEnter : MonoBehaviour
{
    private GameObject objectToEnableScriptsOn;
    [SerializeField] private GameObject[] gatesToClose;

    private void Start()
    {
        SpawnUnitsInBossLevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        closeGate();
        Invoke("enableScripts", 1f);
    }

    private void enableScripts()
    {
        MonoBehaviour[] scripts = objectToEnableScriptsOn.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }

    private void closeGate()
    {
        foreach (GameObject gate in gatesToClose)
        {
            gate.GetComponent<Animator>().SetBool("closeGate", true);
            gate.GetComponent<BoxCollider2D>().enabled = true;
            gate.GetComponent<NavMeshObstacle>().enabled = true;
        }
    }

    private void SpawnUnitsInBossLevel()
    {
        Vector3 playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPosition").transform.position;
        if (GameManager.instance.currentPlayerCharacterString.Equals(GameAssets.i.WASDCharacter.name) || GameManager.instance.currentPlayerCharacterString.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
            playerSpawnPoint.y = -6.9f;

        GameObject playerHolder = Instantiate(GameManager.instance.GetPlayerObjectByName(), playerSpawnPoint, Quaternion.identity);
        playerHolder.GetComponent<PlayerController>().setActivePlayer();


        Vector3 bossSpawnPoint = GameObject.FindGameObjectWithTag("BossSpawnPosition").transform.position;
        switch (SceneManager.GetActiveScene().name)
        {
            case "rotatorBossRoom":
                objectToEnableScriptsOn = Instantiate(GameAssets.i.rotatorBoss, bossSpawnPoint, Quaternion.identity);
                break;
        }



        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().enabled = true;
    }
}
