using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float cooldown;
    [SerializeField] private AudioClip dealDamageClip;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void attack()
    {
        if (canAttack)
        {
            StartCoroutine(melee());
        }
    }

    IEnumerator melee()
    {
        canAttack = false;
        animator.SetTrigger("Melee");

        yield return new WaitForSeconds(0.22f);
        SoundManager.instance.PlaySound(dealDamageClip);
        GameManager.instance.playerReference.GetComponent<PlayerController>().TakeDamage(meleeDamage);

        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
