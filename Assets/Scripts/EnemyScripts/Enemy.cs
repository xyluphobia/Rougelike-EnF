using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CameraShake;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public int maxHealth = 20;
    public int currentHealth;

    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip hitClip;

    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject scoreText;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        ShowText(damageText, damage);
        currentHealth -= damage;

        animator.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            Die();
            ShowText(scoreText, 100);
            GameManager.instance.UpdateScore(100);
        }
        else
            SoundManager.instance.PlaySound(hitClip);
    }

    private void ShowText(GameObject textObject, int input)
    {
        if (textObject)
        {   
            GameObject prefab = Instantiate(textObject, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMeshPro>().text = input.ToString();
        }
    }

    void Die() 
    {
        transform.parent.BroadcastMessage("OnDeath");
        CameraShaker.Presets.ShortShake2D(0.009f, 0.009f, 30, 3);

        SoundManager.instance.PlaySound(deathClip);

        if (GetComponent<EnemyAI>()!= null)
            GetComponent<EnemyAI>().enabled = false;
        if (GetComponent<DamageOnCollision>()!= null)
            GetComponent<DamageOnCollision>().enabled = false;       // Prevent collision damage from dead enemy.

        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(scaleOverTime(this.gameObject.transform, new Vector3(0, 0, 0), 4f));
    }

    bool isScaling = false;

    IEnumerator scaleOverTime(Transform objectToScale, Vector3 toScale, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isScaling)
        {
            yield break; ///exit if this is still running
        }
        isScaling = true;

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }


        isScaling = false;
    }
}
