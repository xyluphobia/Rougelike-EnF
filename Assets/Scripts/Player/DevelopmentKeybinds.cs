using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentKeybinds : MonoBehaviour
{
    private PlayerController player;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }


    private void OnDevelopment_ForceBossLevel()
    {
        if (!GameManager.instance.ForceBossRoomNext)
        {
            GameManager.instance.ForceBossRoomNext = true;
            Debug.Log("Next floor is boss floor: On");
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
        player.healthText.text = "Health: " + player.maxHealth;
        Debug.Log("You are fully healed.");
    }

    private void OnDevelopment_RechargeDash()
    {
        player.canDash = true;
        Debug.Log("Dash reset.");
    }
}
