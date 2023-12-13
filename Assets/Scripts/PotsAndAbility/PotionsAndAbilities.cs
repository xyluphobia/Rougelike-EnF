using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PotionsAndAbilities : MonoBehaviour
{
    protected float abilityTimer;
    [SerializeField] protected float cooldown;
    [SerializeField] protected float duration;

    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public SpeedPotion speedPotion;
    [HideInInspector] public HealthPotion healthPotion;
    
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        speedPotion = playerController.GetComponent<SpeedPotion>();
        healthPotion = playerController.GetComponent<HealthPotion>();
    }

    public void PickupPotion()
    {
        SoundManager.instance.RandomizeSfx(GameAssets.i.potionPickupClips);
    }
}
