using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevelopmentKeybinds : MonoBehaviour
{
    private PlayerController player;
    private WASDCharacter wasdPlayerScript;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }


    private void OnDevelopment_ForceBossLevel()
    {
        if (!GameManager.instance.ForceBossRoomNext)
        {
            GameManager.instance.setPlayerForNextLevel(player.health, gameObject);
            GameManager.instance.ForceBossRoomNext = true;
            Debug.Log("Next floor is boss floor: On");

            SceneManager.LoadScene(2);
        }
        else
        {
            GameManager.instance.ForceBossRoomNext = false;
            Debug.Log("Next floor is boss floor: Off");
        }
    }

    private void OnDevelopment_Invincibility()
    {
        if (!player.invulnerable)
        {
            player.invulnerable = true;
            Debug.Log("Invulnerable: On");
        }
        else
        {
            player.invulnerable = false;
            Debug.Log("Invulnerable: Off");
        }
    }

    private void OnDevelopment_KillAllEnemies()
    {
        GameObject[] objectsWithEnemyTag = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject potentialEnemy in objectsWithEnemyTag)
        {
#nullable enable
            Enemy? enemyScript = potentialEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(999999);
            }
#nullable disable
        }
        Debug.Log("Killed all enemies.");
    }

    private void OnDevelopment_FullHeal()
    {
        player.health = player.maxHealth;
        Debug.Log("You are fully healed.");
    }

    private void OnDevelopment_RechargeDash()
    {
        wasdPlayerScript.canDash = true;
        Debug.Log("Dash reset.");
    }
}
