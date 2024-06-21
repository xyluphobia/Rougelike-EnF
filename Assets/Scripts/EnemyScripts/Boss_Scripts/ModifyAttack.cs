using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyAttack : MonoBehaviour
{
    private Rigidbody2D rb;
    private RangedShot rangedShot;
    private Animator animator;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rangedShot = gameObject.GetComponent<RangedShot>();
        animator = gameObject.GetComponent<Animator>();
    }

    // pick which attack to use based on the monster this is attached too, cast it through a standard 'modified attack' function which can be called through ranged shot
    public void TriggerModifiedAttack()
    {
        switch (gameObject.name)
        {
            case "Rotator":
                StartCoroutine(RotatorAttackPattern());
                break;
        }
    }


    private IEnumerator RotatorAttackPattern()
    {
        Debug.Log("Rotator!");
        /* Variables used only in this pattern. */
        bool rotating = false;
        bool firstCycle = true;
        float shotAnimationTime = 0.3f;
        Enemy enemyScript = gameObject.GetComponent<Enemy>();
        GameObject standardProjectile = GameAssets.i.standardProjectile;
        GameObject trackingProjectile = GameAssets.i.trackingProjectile;
        GameObject rotatorProjectile = GameAssets.i.rotatorProjectile;

        Transform AttackPositionUp = transform.Find("AttackUp"); 
        Transform AttackPositionDown = transform.Find("AttackDown");
        Transform AttackPositionLeft = transform.Find("AttackLeft");
        Transform AttackPositionRight = transform.Find("AttackRight");

        Transform AttackPositionUpLine1 = transform.Find("P2Up1");
        Transform AttackPositionUpLine2 = transform.Find("P2Up2");
        Transform AttackPositionUpLine3 = transform.Find("P2Up3");

        Transform AttackPositionDownLine1 = transform.Find("P2Down1");
        Transform AttackPositionDownLine2 = transform.Find("P2Down2");
        Transform AttackPositionDownLine3 = transform.Find("P2Down3");

        Transform AttackPositionLeftLine1 = transform.Find("P2Left1");
        Transform AttackPositionLeftLine2 = transform.Find("P2Left2");
        Transform AttackPositionLeftLine3 = transform.Find("P2Left3");

        Transform AttackPositionRightLine1 = transform.Find("P2Right1");
        Transform AttackPositionRightLine2 = transform.Find("P2Right2");
        Transform AttackPositionRightLine3 = transform.Find("P2Right3");

        // Spin 3 times damaging in a proximity
        StartCoroutine(rotateOverTime(3));
        yield return new WaitForSeconds(4.5f);  // need to add damage in aoe

        while (enemyScript.currentHealth > 0)
        {
            /* ~~ Phase 1 ~~ */
            // Spin once damaging in proximity | This is skipped on the first rotation
            if (!firstCycle)
            {
                StartCoroutine(rotateOverTime(1));
                yield return new WaitForSeconds(1.5f);  // need to add damage in aoe
            }

            // Shoot 2 projectiles at once spawning on two different random positions (NSEW)
            List<int> nums = new List<int> { 1, 2, 3, 4 };
            int spawn1 = nums[Random.Range(0, nums.Count - 1)];
            nums.Remove(spawn1);
            int spawn2 = nums[Random.Range(0, nums.Count - 1)];
            nums.Remove(spawn2);
        
            yield return new WaitForSeconds(1f);

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

            /* ~~ Phase 2 ~~ */
            // Spin once damaging in proximity
            StartCoroutine(rotateOverTime(1));
            yield return new WaitForSeconds(1.5f);  // need to add damage in aoe

            // Shoot 3 projectiles spawning in a line which move toward the players location at the time of firing forming a triangle
            yield return new WaitForSeconds(1f);
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

            /* ~~ Phase 3 ~~ */
            // Spin twice damaging in proximity
            StartCoroutine(rotateOverTime(2));
            yield return new WaitForSeconds(3f);  // need to add damage in aoe

            // Shoot 1 special projectile which rotates the players controls (previous control used to move 'Up' now moves you 'Right' etc) U>R, R>D, D>L, L>U
            rangedShot.EnemyShoot(rotatorProjectile, null, shotAnimationTime);
            yield return new WaitForSeconds(2.5f);

            firstCycle = false;
        }

        /* ~~ Out Of Phase ~~ */
        IEnumerator rotateOverTime(int rotationsRemaining)
        {
            while (rotationsRemaining > 0)
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
        }
    }
}
