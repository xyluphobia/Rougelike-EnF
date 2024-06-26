using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatorPattern : MonoBehaviour
{
    private Rigidbody2D rb;
    private RangedShot rangedShot;
    private Animator animator;
    private Enemy enemyScript;
    private EnemyAI enemyAi;
    private DamageOnCollision damageOnCollision;
    private CircleCollider2D circleCollider;

    bool rotating = false;
    bool firstCycle = true;
    bool swapAngle = false;
    readonly float shotAnimationTime = 0.3f;
    private string originalBinds;

    [Header("Spin/Swirl")]
    [SerializeField] AudioClip[] spinAudio;
    [SerializeField] GameObject swirlEffect;

    [Header("Projectiles")]
    [SerializeField] private GameObject standardProjectile;
    [SerializeField] private GameObject trackingProjectile;
    [SerializeField] private GameObject rotatorProjectile;

    [Header("Basic Attack Positions")]
    [SerializeField] private Transform AttackPositionUp;
    [SerializeField] private Transform AttackPositionDown;
    [SerializeField] private Transform AttackPositionLeft;
    [SerializeField] private Transform AttackPositionRight;

    [Header("Special Attack Positions")]
    [SerializeField] private Transform SpecialAttackPositionUp;
    [SerializeField] private Transform SpecialAttackPositionDown;
    [SerializeField] private Transform SpecialAttackPositionLeft;
    [SerializeField] private Transform SpecialAttackPositionRight;

    [Header("Attack Positions Line Up")]
    [SerializeField] private Transform AttackPositionUpLine1;
    [SerializeField] private Transform AttackPositionUpLine2;
    [SerializeField] private Transform AttackPositionUpLine3;

    [Header("Attack Positions Line Down")]
    [SerializeField] private Transform AttackPositionDownLine1;
    [SerializeField] private Transform AttackPositionDownLine2;
    [SerializeField] private Transform AttackPositionDownLine3;

    [Header("Attack Positions Line Left")]
    [SerializeField] private Transform AttackPositionLeftLine1;
    [SerializeField] private Transform AttackPositionLeftLine2;
    [SerializeField] private Transform AttackPositionLeftLine3;

    [Header("Attack Positions Line Right")]
    [SerializeField] private Transform AttackPositionRightLine1;
    [SerializeField] private Transform AttackPositionRightLine2;
    [SerializeField] private Transform AttackPositionRightLine3;

    [Header("Phase 2 Attack Positions")]
    [SerializeField] private Transform[] Phase2AttackPositions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rangedShot = GetComponent<RangedShot>();
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<Enemy>();
        enemyAi = GetComponent<EnemyAI>();
        damageOnCollision = GetComponent<DamageOnCollision>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void BindHolder(string originalBindsPassthrough)
    {
        if (originalBinds != null)
            return;

        originalBinds = originalBindsPassthrough;
    }

    private void OnDeath()
    {
        if (originalBinds == null) 
            return;

        originalBinds = originalBinds.Remove(originalBinds.Length - 1, 1).Remove(0, 14);

        string[] splitInputs = originalBinds.Split(",");
        char wasdUp = splitInputs[0][^1];
        char wasdDown = splitInputs[1][^1];
        char wasdLeft = splitInputs[2][^1];
        char wasdRight = splitInputs[3][^1];

        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(1, $"<Keyboard>/{wasdUp}"); // wasdUp
        playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(2, $"<Keyboard>/{wasdDown}"); // wasdDown
        playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(3, $"<Keyboard>/{wasdLeft}"); // wasdLeft
        playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(4, $"<Keyboard>/{wasdRight}"); // wasdRight
    }

    public IEnumerator RotatorAttackPattern()
    { 
        // Spin 3 times damaging in a proximity
        StartCoroutine(rotateOverTime(3, 2));
        yield return new WaitForSeconds(2f);  // need to add damage in aoe

        while (enemyScript.currentHealth > 200)
        {
            /* ~~ Attack 1 ~~ */
            // Spin once damaging in proximity | This is skipped on the first rotation
            if (!firstCycle)
            {
                StartCoroutine(rotateOverTime(1, 0));
                yield return new WaitForSeconds(1.5f);  // need to add damage in aoe
            }

            // Shoot 2 projectiles at once spawning on two different random positions (NSEW)
            List<int> nums = new List<int> { 1, 2, 3, 4 };
            int spawn1 = nums[Random.Range(0, nums.Count - 1)];
            nums.Remove(spawn1);
            int spawn2 = nums[Random.Range(0, nums.Count - 1)];
            nums.Remove(spawn2);

            yield return new WaitForSeconds(1f);
            if (enemyScript.currentHealth < 200) break;

            bool numFound = false;
            if (nums.Contains(1))
            {
                numFound = true;
                rangedShot.EnemyShoot(trackingProjectile, AttackPositionUp, shotAnimationTime);
                yield return new WaitForSeconds(shotAnimationTime);
            }
            if (nums.Contains(2))
            {
                if (numFound)
                    rangedShot.EnemyShoot(trackingProjectile, AttackPositionDown, 0);
                else
                {
                    numFound = true;
                    rangedShot.EnemyShoot(trackingProjectile, AttackPositionDown, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                }
            }
            if (nums.Contains(3))
            {
                if (numFound)
                    rangedShot.EnemyShoot(trackingProjectile, AttackPositionLeft, 0);
                else
                {
                    numFound = true;
                    rangedShot.EnemyShoot(trackingProjectile, AttackPositionLeft, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                }
            }
            if (nums.Contains(4))
            {
                rangedShot.EnemyShoot(trackingProjectile, AttackPositionRight, 0);
            }
            yield return new WaitForSeconds(1.5f);

            /* ~~ Attack 2 ~~ */
            // Spin once damaging in proximity
            StartCoroutine(rotateOverTime(1, 0));
            yield return new WaitForSeconds(1.5f);  // need to add damage in aoe

            // Shoot 3 projectiles spawning in a line which move toward the players location at the time of firing forming a triangle
            yield return new WaitForSeconds(1f);
            if (enemyScript.currentHealth < 200) break;

            Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            switch (rangedShot.findDirection(direction).name)
            {
                case "AttackUp":
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionUpLine1, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionUpLine2, 0);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionUpLine3, 0);
                    break;

                case "AttackDown":
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionDownLine1, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionDownLine2, 0);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionDownLine3, 0);
                    break;

                case "AttackLeft":
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionLeftLine1, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionLeftLine2, 0);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionLeftLine3, 0);
                    break;

                case "AttackRight":
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionRightLine1, shotAnimationTime);
                    yield return new WaitForSeconds(shotAnimationTime);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionRightLine2, 0);
                    rangedShot.EnemyShoot(standardProjectile, AttackPositionRightLine3, 0);
                    break;
            }
            yield return new WaitForSeconds(2f);

            /* ~~ Attack 3 ~~ */
            // Spin twice damaging in proximity
            StartCoroutine(rotateOverTime(2, 1));
            yield return new WaitForSeconds(3f);  // need to add damage in aoe
            if (enemyScript.currentHealth < 200) break;

            // Shoot 1 special projectile which rotates the players controls (previous control used to move 'Up' now moves you 'Right' etc) U>R, R>D, D>L, L>U
            Vector2 dir = enemyAi.movement;
            if (dir.x > 0.7)
            {
                rangedShot.EnemyShoot(rotatorProjectile, SpecialAttackPositionRight, shotAnimationTime, true);
            }
            else if (dir.x < -0.7)
            {
                rangedShot.EnemyShoot(rotatorProjectile, SpecialAttackPositionLeft, shotAnimationTime, true);
            }
            else if (dir.y > 0.7)
            {
                rangedShot.EnemyShoot(rotatorProjectile, SpecialAttackPositionUp, shotAnimationTime, true);
            }
            else if (dir.y < -0.7)
            {
                rangedShot.EnemyShoot(rotatorProjectile, SpecialAttackPositionDown, shotAnimationTime, true);
            }
            else
            {
                rangedShot.EnemyShoot(rotatorProjectile, SpecialAttackPositionDown, shotAnimationTime, true);
            }

            yield return new WaitForSeconds(2f);

            firstCycle = false;
        }

        /* ~~ Phase 2 ~~ */
        trackingProjectile.GetComponent<RangedShotTrackingProjectile>().destroyProjectileAfter = 4f;
        trackingProjectile.GetComponent<RangedShotTrackingProjectile>().trackingTime = 2.5f;
        rotatorProjectile.GetComponent<RangedShotTrackingProjectile>().destroyProjectileAfter = 4f;
        standardProjectile.GetComponent<RangedShotStandardProjectile>().destroyProjectileAfter = 5f;

        while (enemyScript.currentHealth <= 200 && enemyScript.currentHealth > 0)
        {
            /* ~~ Attack 1 ~~ */
            StartCoroutine(rotateOverTime(2, 0));
            //yield return new WaitForSeconds(3f);

            rangedShot.EnemyShoot(trackingProjectile, Phase2AttackPositions[1], 0);
            //yield return new WaitForSeconds(shotAnimationTime);
            rangedShot.EnemyShoot(trackingProjectile, Phase2AttackPositions[2], 0);
            rangedShot.EnemyShoot(trackingProjectile, Phase2AttackPositions[8], 0);
            rangedShot.EnemyShoot(trackingProjectile, Phase2AttackPositions[9], 0);

            yield return new WaitForSeconds(1.75f + 1.5f);

            /* ~~ Attack 2 ~~ */
            StartCoroutine(rotateOverTime(2, 0));
            //yield return new WaitForSeconds(3f);

            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[13], 0);
            //yield return new WaitForSeconds(shotAnimationTime);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[0], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[3], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[4], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[6], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[7], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[10], 0);
            rangedShot.EnemyShoot(standardProjectile, Phase2AttackPositions[11], 0);

            yield return new WaitForSeconds(1.75f + 1.5f);
            /* ~~ Attack 3 ~~ */

            StartCoroutine(rotateOverTime(3, 0));
            //yield return new WaitForSeconds(3f);

            if (swapAngle)
            {
                swapAngle = false;
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[5], 0, true);
                //yield return new WaitForSeconds(shotAnimationTime);
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[11], 0, true);
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[13], 0, true);
            }
            else
            {
                swapAngle = true;
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[12], 0, true);
                //yield return new WaitForSeconds(shotAnimationTime);
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[4], 0, true);
                rangedShot.EnemyShoot(rotatorProjectile, Phase2AttackPositions[6], 0, true);
            }

            yield return new WaitForSeconds(1.75f + 1.5f);
        }

        /* ~~ Out Of Phase ~~ */
        IEnumerator rotateOverTime(int rotationsRemaining, int indexOfSwirl)
        {
            SoundManager.instance.PlaySound(spinAudio[indexOfSwirl]);
            swirlEffect.SetActive(true);
            StartCoroutine(FadeSwirlEffect(new Color32(255, 255, 255, 255), 3f));
            enemyScript.invulnerable = true;
            circleCollider.enabled = true;
            damageOnCollision.enabled = true;
            enemyAi.chasePlayer = true;

            while (rotationsRemaining > 0 && enemyScript.currentHealth > 0)
            {
                if (rotating)
                {
                    yield break;
                }
                rotating = true;
                animator.SetBool("IsSpinning", true);

                Vector3 newRot = gameObject.transform.eulerAngles + new Vector3(0, 0, 360);
                Vector3 currentRot = gameObject.transform.eulerAngles;

                float counter = 0;
                float duration = 1.1f;
                while (counter < duration)
                {
                    counter += Time.deltaTime;
                    gameObject.transform.eulerAngles = Vector3.Lerp(currentRot, newRot, counter / duration);
                    yield return null;
                }

                rotationsRemaining -= 1;
                rotating = false;
                animator.SetBool("IsSpinning", false);
            }

            enemyAi.chasePlayer = false;
            StartCoroutine(FadeSwirlEffect(new Color32(255, 255, 255, 0), 3f));
            swirlEffect.SetActive(false);
            enemyScript.invulnerable = false;
            circleCollider.enabled = false;
            damageOnCollision.enabled = false;


            IEnumerator FadeSwirlEffect(Color goalColor, float speed)
            {
                Color startColor = swirlEffect.GetComponent<SpriteRenderer>().color;

                float tick = 0f;
                while (swirlEffect.GetComponent<SpriteRenderer>().color != goalColor)
                {
                    tick += Time.deltaTime * speed;
                    swirlEffect.GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, goalColor, tick);
                    yield return null;
                }
            }
        }
    }
}
