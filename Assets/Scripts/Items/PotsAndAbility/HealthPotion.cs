using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PotionsAndAbilities
{
    const float defaultHealPercentage = 25f;

    public void useHealthPotion(float healPercentage = defaultHealPercentage)
    {
        playerController = GameManager.instance.playerReference.GetComponent<PlayerController>();
        PickupPotion();

        GameManager.instance.UpdateScore(25);

        int healAmount = Mathf.RoundToInt(playerController.maxHealth * healPercentage);
        playerController.HealDamage(healAmount);

        transform.parent.gameObject.SetActive(false);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            useHealthPotion();
        }
    }
}
