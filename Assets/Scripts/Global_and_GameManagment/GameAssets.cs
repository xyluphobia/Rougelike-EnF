using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets i
    { 
        get 
        { 
            if (_instance == null)
                _instance = Instantiate(Resources.Load("GameAssets") as GameObject).GetComponent<GameAssets>();

            return _instance; 
        } 
    }
    public GameObject AbilityBarActive;
    public GameObject AbilityBarWASD;
    public GameObject AbilityBarMOBA;
    private void Awake()
    {
        AbilityBarWASD = GameObject.Find("AbilityBarWASD");
        AbilityBarMOBA = GameObject.Find("AbilityBarMOBA");
        AbilityBarWASD.SetActive(false);
        AbilityBarMOBA.SetActive(false);
    }

    [Header("Player/Important")]
    public GameObject player;
    public GameObject exit;
    public GameObject movementIndicatorArrow;

    [Header("MOBA Character")]
    public GameObject lightningBoltProjectile;
    public GameObject icicleProjectile;

    [Header("Popup Texts")]
    public GameObject damageText;
    public GameObject scoreText;
    public GameObject healthChangeText;
    public GameObject scoreChangeText;

    [Header("Audio")]
    public AudioClip coinPickupClip;
    public AudioClip chestOpenClip;
    public AudioClip chestCloseClip;
    public AudioClip[] potionPickupClips;

    [Header("Intantiated Objects Holders")]
    public GameObject EnemyHolder;
    public GameObject PropHolder;
    public GameObject ItemHolder;
}
