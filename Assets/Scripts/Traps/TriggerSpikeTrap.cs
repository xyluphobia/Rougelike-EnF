using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikeTrap : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int damage = 50;
    [SerializeField] private AudioClip trapTriggeredClip;
    private bool playerOnTrap = false;
    private bool trapArmed = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!trapArmed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnTrap = true;
            StartCoroutine(ActiveTrap(collision.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnTrap = false;
        }
    }



    IEnumerator ActiveTrap(GameObject player)
    {
        animator.SetTrigger("Alerted");
        yield return new WaitForSeconds(1.5f);

        if (playerOnTrap)
        {
            SoundManager.instance.PlaySound(trapTriggeredClip);
            animator.SetTrigger("Activated");
            player.GetComponent<PlayerController>().TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }

        trapArmed = false;
        animator.SetTrigger("Disarm");
        yield return new WaitForSeconds(0.1f);
        trapArmed = true;
    }
}
