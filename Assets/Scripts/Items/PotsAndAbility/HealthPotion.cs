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

        int healAmount = Mathf.RoundToInt(playerController.maxHealth * healPercentage);
        playerController.HealDamage(healAmount);
    }
}
