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
    [Header("Player/Important")]
    public GameObject player;
    public GameObject exit;

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

}
