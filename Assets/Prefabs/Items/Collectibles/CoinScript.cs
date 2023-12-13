using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] int ScoreValue = 20;


    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        SoundManager.instance.PlaySound(GameAssets.i.coinPickupClip);

        GameManager.instance.ShowText(GameAssets.i.scoreText, ScoreValue, gameObject);
        GameManager.instance.UpdateScore(ScoreValue);
        
        Destroy(gameObject, 0.05f);
    }
}
