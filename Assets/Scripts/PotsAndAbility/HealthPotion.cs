using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PotionsAndAbilities
{
    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void useHealthPotion(float healPercentage)
    {
        PickupPotion();

        GameManager.instance.UpdateScore(25);

        int health = playerController.health;
        int maxHealth = playerController.maxHealth;

        int healthIncreaseBy = Mathf.RoundToInt(maxHealth * healPercentage);
        health += healthIncreaseBy;
        GameManager.instance.TextChangeVisualizer(false, "+" + healthIncreaseBy.ToString());

        if (health > maxHealth)
            health = maxHealth;

        playerController.health = health;
        playerController.healthText.text = "Health: " + health;
    }
}
