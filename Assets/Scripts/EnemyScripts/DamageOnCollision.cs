using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public int damage = 25;

    [SerializeField] private AudioClip[] dealDamageClips;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            SoundManager.instance.RandomizeSfx(dealDamageClips);
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
