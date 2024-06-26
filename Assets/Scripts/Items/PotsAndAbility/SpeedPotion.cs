using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : PotionsAndAbilities
{
    [SerializeField] private int boostPercentage;
    private float boostAsPercent;
    
    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        boostAsPercent = (100f + boostPercentage) / 100f;
    }

    public void useSpeedPotion() 
    {
        PickupPotion();

        GameManager.instance.UpdateScore(25);

        if (Time.time >= abilityTimer)
        {
            speedPotionEffect();
            abilityTimer = Time.time + cooldown;
        }
    }

    private void speedPotionEffect()
    {
        playerController.Boost(boostAsPercent);

        Invoke("resetSpeedPotionEffect", duration);
    }

    private void resetSpeedPotionEffect()
    {
        playerController.ResetBoost(boostAsPercent);
    }
}
