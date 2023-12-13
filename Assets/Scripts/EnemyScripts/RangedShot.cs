using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedShot : MonoBehaviour
{
    [SerializeField] private AudioClip[] projectileShotClips;

    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyAI enemyAi;
    [SerializeField] private GameObject projectile;

    private float currentShotCooldown;
    [SerializeField] private float firstShotAfterTime;
    [SerializeField] private float ShotsPerSecond;

    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointLeft;
    private Transform attackPoint;

    private Animator animator;

    void Start()
    {
        currentShotCooldown = firstShotAfterTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (enemyAi.isInAttackRange && enemy.currentHealth > 0)
        {
            if (currentShotCooldown <= 0)
            {
                EnemyShoot();
            }
            else
            {
                currentShotCooldown -= Time.deltaTime;
            }
        }
    }

    private void EnemyShoot()
    {
         SoundManager.instance.RandomizeSfx(projectileShotClips);

        currentShotCooldown = ShotsPerSecond;

        Vector3 dir = enemyAi.movement;
        if (dir.x > 0.7)
        {
            attackPoint = attackPointRight;
        } 
        else if (dir.x < -0.7)
        {
            attackPoint = attackPointLeft;
        }
        else if (dir.y > 0.7)
        {
            attackPoint = attackPointUp;
        }
        else if (dir.y < -0.7) 
        {
            attackPoint = attackPointDown;
        }

        animator.SetFloat("attackH", dir.x);
        animator.SetFloat("attackV", dir.y);
        animator.SetTrigger("Shoot");

        StartCoroutine(ShootAfterAnimation());
    }

    IEnumerator ShootAfterAnimation()
    {
        yield return new WaitForSeconds(0.22f);
        Instantiate(projectile, attackPoint.position, gameObject.transform.rotation);
    }
}
