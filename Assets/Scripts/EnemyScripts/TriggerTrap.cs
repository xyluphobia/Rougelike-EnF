using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrap : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int damage = 50;
    [SerializeField] private AudioClip trapTriggeredClip;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ActiveTrap();
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }


    public void ActiveTrap()
    {
        SoundManager.instance.PlaySound(trapTriggeredClip);
        animator.SetTrigger("TriggerTrap");
    }
}
