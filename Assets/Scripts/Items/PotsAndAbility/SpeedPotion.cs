using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : PotionsAndAbilities
{
    [SerializeField] private int boostPercentage;
    private float boostAsPercent;
    
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        boostAsPercent = (100f + boostPercentage) / 100f;
    }

    public void useSpeedPotion() 
    {
        PickupPotion();
        speedPotionEffect();

        GameManager.instance.UpdateScore(25);
        transform.parent.gameObject.SetActive(false);
    }

    private void speedPotionEffect()
    {
        playerController.Boost(boostAsPercent);

        Invoke("resetSpeedPotionEffect", duration);
    }

    private void resetSpeedPotionEffect()
    {
        playerController.ResetBoost();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            useSpeedPotion();
        }
    }
}
