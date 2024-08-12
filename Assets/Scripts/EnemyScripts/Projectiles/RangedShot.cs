using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedShot : MonoBehaviour
{
    [SerializeField] private AudioClip[] projectileShotClips;

    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyAI enemyAi;
    [SerializeField] private GameObject projectile;

    private float currentShotCooldown;
    [SerializeField] private float firstShotAfterTime;
    [SerializeField] private float ShotEveryXSeconds;

    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointLeft;

    private Animator animator;
    [SerializeField] private float waitXForAnim;

    [SerializeField] private ModifyAttack modifierScript = null;
    private bool modifierTriggered = false;

    void Start()
    {
        currentShotCooldown = firstShotAfterTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemyAi.isInAttackRange && enemy.currentHealth > 0)
        {
            if (modifierScript != null)
            {
                if (modifierTriggered)
                    return;

                modifierTriggered = true;
                modifierScript.TriggerModifiedAttack();
            }
            else
            {
                if (currentShotCooldown <= 0)
                {
                    currentShotCooldown = ShotEveryXSeconds;
                    EnemyShoot(projectile);
                }

                else
                    currentShotCooldown -= Time.deltaTime;
            }
        }
    }

    public void EnemyShoot(GameObject projectileToShoot, Transform attackPoint = null, float waitTimeToPass = -1f, bool isSpecialAttack = false)
    {
        Vector2 dir = enemyAi.movement;
        if (attackPoint == null)
            attackPoint = findDirection(dir);


        float passingWaitDuration;
        if (waitTimeToPass == -1f)
        {
            passingWaitDuration = waitXForAnim;
        }
        else
            passingWaitDuration = waitTimeToPass;

        if (passingWaitDuration != 0f)
        {
            SoundManager.instance.RandomizeSfx(projectileShotClips);

            if (isSpecialAttack)
                animator.SetTrigger("SpecialAttack");
            else
                animator.SetTrigger("Shoot");
        }
        StartCoroutine(ShootAfterAnimation(projectileToShoot, attackPoint.position, passingWaitDuration));
    }

    private IEnumerator ShootAfterAnimation(GameObject projectileToShootFinal, Vector3 attackPointPosition, float waitTime)
    {
        enemyAi.canMove = false;
        yield return new WaitForSeconds(waitTime);
        Instantiate(projectileToShootFinal, attackPointPosition, gameObject.transform.rotation);
        enemyAi.canMove = true;
    }

    public Transform findDirection(Vector2 dir)
    {
        Transform attackPoint;

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

        else
        {
            attackPoint = attackPointDown;
        }

        return attackPoint;
    }
}
