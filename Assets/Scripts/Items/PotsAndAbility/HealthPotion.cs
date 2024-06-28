using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PotionsAndAbilities
{
    const float defaultHealPercentage = 25f;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void useHealthPotion(float healPercentage = defaultHealPercentage)
    {
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
