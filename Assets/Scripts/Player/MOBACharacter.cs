using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MOBACharacter : MonoBehaviour
{
    private PlayerController playerController;
    private NavMeshAgent navAgent;

    private GameObject movementIndicatorArrow;
    private Image[] AbilityBarImageHolder;
    private Dictionary<string, Image> AbilityImagesDict = new();
    private bool disableMovement = false;
    private bool canCast = true;
    [SerializeField] private AudioClip fizzledCastSfx;

    [SerializeField] private AudioClip lightningCastSfx;
    private bool lightningIsOnCooldown = false;
    private float lightningCooldown = 1.5f;

    [SerializeField] private AudioClip icicleCastSfx;
    private bool icicleIsOnCooldownRapid = false;
    private float icicleCooldownRapid = 0.5f;
    private float icicleCooldownGeneral = 3f;
    private int icicleRapidShotsUsed = 0;
    private Coroutine activeUITimer = null;
    private Coroutine activeGeneralIcicleTimer = null;
    [HideInInspector] public Dictionary<GameObject, int> enemyFreezeCounters = new();

    [SerializeField] private AudioClip teleportStartSfx;
    [SerializeField] private AudioClip teleportEndSfx;
    private bool teleportIsOnCooldown = false;
    private float teleportCooldown = 5f;

    private Vector2 clickedPos;
    private Vector2 lastFramePos;
    private Vector2 currentFramePos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        lastFramePos = transform.position;

        AbilityBarImageHolder = GameAssets.i.AbilityBarMOBA.GetComponentsInChildren<Image>(true);
        foreach (Image img in AbilityBarImageHolder)
        {
            AbilityImagesDict.Add(img.transform.name, img);
        }
    }

    private void FixedUpdate() // could find direction by saving previous step and comparing against current position then checking which direction was travelled
    {
        currentFramePos = transform.position;

        if (currentFramePos != lastFramePos)
        {
            Vector2 diffVector = new Vector2(System.Math.Abs(currentFramePos.x - lastFramePos.x), System.Math.Abs(currentFramePos.y - lastFramePos.y));

            float diffXPercent = diffVector.x / (diffVector.x + diffVector.y);
            float diffYPercent = diffVector.y / (diffVector.x + diffVector.y);

            if (currentFramePos.x < lastFramePos.x)
                diffXPercent *= -1;
            if (currentFramePos.y < lastFramePos.y)
                diffYPercent *= -1;

            playerController.animator.SetFloat("Horizontal", diffXPercent);
            playerController.animator.SetFloat("Vertical", diffYPercent);
            playerController.animator.SetFloat("Speed", diffVector.sqrMagnitude);

            lastFramePos = currentFramePos;
        }
        else
        {
            playerController.animator.SetFloat("Speed", 0);
        }
    }

    public void TakeControl()
    {
        movementIndicatorArrow = Instantiate(GameAssets.i.movementIndicatorArrow);
        movementIndicatorArrow.transform.localScale = Vector3.zero;

        playerController.SetActiveAbilityBar(GameAssets.i.AbilityBarMOBA);
    }

    public void GiveControl()
    {
        Destroy(movementIndicatorArrow);
    }

    private void OnMove_MOBA()
    {
        if (disableMovement) return;

        clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        navAgent.SetDestination(new Vector3(clickedPos.x, clickedPos.y));

        movementIndicatorArrow.transform.localScale = Vector3.one;
        movementIndicatorArrow.transform.position = navAgent.destination;
    }

    /* Abilities */
    private void OnAbilityOne() /* |Q| Basic Damage Ability ~ Lightning Bolt */
    {
        if (lightningIsOnCooldown || !canCast) return;

        canCast = false;
        StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["QTimer"], lightningCooldown));

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(mousePos, 2f, (Vector2)transform.position);
        foreach (RaycastHit2D enemy in nearbyEnemies)
        {
            if (enemy.transform.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(enemy.point, mousePos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            StartCoroutine(HoldCastPositionForSeconds(0.25f));
            StartCoroutine(LightningAbilityCooldown());
            SoundManager.instance.PlaySound(lightningCastSfx);

            GameObject bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position, Quaternion.identity);
            bolt.GetComponent<LightningProjectile>().spawnedByPlayer = true;
            bolt.GetComponent<LightningProjectile>().target = closestEnemy;
        }
        else
        {
            StartCoroutine(HoldCastPositionForSeconds(0.25f, true, "LightningBolt"));
            SoundManager.instance.PlaySound(fizzledCastSfx);

            canCast = true;
        }

        IEnumerator LightningAbilityCooldown()
        {
            lightningIsOnCooldown = true;
            yield return new WaitForSeconds(lightningCooldown);
            lightningIsOnCooldown = false;
        }
    }

    private void OnAbilityTwo() /* |W| Interesting Ability ~ Icicle */
    {
        // Target is slowed stacking with each hit until 3 hits where the target is frozen completely
        // Fires towards mouse NO TRACKING
        // Allows 3 activations using the rapid cooldown amount, after these activations no more casts are allowed until the general cooldown is finished
        // The general cooldown starts after each of the 3 activations so if the timer elapses before 3 casts are used the ability is reset fully to 0 rapid fires used

        if (icicleIsOnCooldownRapid || !canCast) return;

        canCast = false;
        if (icicleRapidShotsUsed < 3)
        {
            StopCoroutineIfNotNull(activeUITimer);
            StopCoroutineIfNotNull(activeGeneralIcicleTimer);

            activeGeneralIcicleTimer = StartCoroutine(IcicleGeneralAbilityCooldown());
            StartCoroutine(HoldCastPositionForSeconds(0.25f));

            GameObject icicle = Instantiate(GameAssets.i.icicleProjectile, transform.position, Quaternion.identity);
            if (icicleRapidShotsUsed != 2) /* First Two Shots ~ Slows 30% | 60% */
            {
                activeUITimer = StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["WTimer"], icicleCooldownRapid));
                StartCoroutine(IcicleRapidAbilityCooldown());

                if (icicleRapidShotsUsed == 1)
                {
                    icicle.transform.localScale *= 1.15f;
                    icicle.GetComponent<IcicleProjectile>().projectileSpeed = 6f;
                }
            }
            else /* Third shot ~ Freezes 100% (No Movement & No Attacks) */
            {
                activeUITimer = StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["WTimer"], icicleCooldownGeneral));
                icicle.transform.localScale *= 1.3f;
                icicle.GetComponent<IcicleProjectile>().projectileSpeed = 7f;
            }
            icicle.GetComponent<IcicleProjectile>().SetDirection(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // spawn projectile
            // scale the projectile up based on which shot it is making the 3rd one largest
            icicleRapidShotsUsed += 1;
        }
        canCast = true;

        IEnumerator IcicleGeneralAbilityCooldown()
        {
            yield return new WaitForSeconds(icicleCooldownGeneral);
            icicleRapidShotsUsed = 0;
        }
        IEnumerator IcicleRapidAbilityCooldown()
        {
            icicleIsOnCooldownRapid = true;
            yield return new WaitForSeconds(icicleCooldownRapid);
            icicleIsOnCooldownRapid = false;
        }
    }

    private void OnAbilityThree() /* |E| Movement Ability ~ Teleport */
    {
        if (teleportIsOnCooldown || !canCast) return;

        canCast = false;
        StartCoroutine(TeleportAbilityCooldown());
        StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["ETimer"], teleportCooldown));


        disableMovement = true;
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
        movementIndicatorArrow.transform.localScale = Vector3.zero;

        Vector2 teleportLocaiton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        StartCoroutine(TeleportingAnimationHandler());

        IEnumerator TeleportingAnimationHandler()
        {
            playerController.animator.SetBool("Teleporting", true);
            SoundManager.instance.PlaySound(teleportStartSfx);
            yield return new WaitForSeconds(0.65f);  // Anim time is: 0.417s

            gameObject.transform.position = teleportLocaiton;

            playerController.animator.SetBool("Teleporting", false);
            SoundManager.instance.PlaySound(teleportEndSfx);
            yield return new WaitForSeconds(0.333f);
            disableMovement = false;
            canCast = true;
        }

        IEnumerator TeleportAbilityCooldown()
        {
            teleportIsOnCooldown = true;
            yield return new WaitForSeconds(teleportCooldown);
            teleportIsOnCooldown = false;
        }
    }

    private void OnUltimateAbility() /* |R| Ultimate Ability */
    {

    }

    IEnumerator HoldCastPositionForSeconds(float secondsToHold, bool fizzling = false, string fizzledProjectile = "")
    {
        disableMovement = true;

        Dictionary<string, float> mouseDirections = Tools.instance.FindDirectionOfMouseFromPlayer(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform);
        playerController.animator.SetFloat("attackH", mouseDirections["AttackH"]);
        playerController.animator.SetFloat("attackV", mouseDirections["AttackV"]);

        if (fizzling)
        {
            fizzlingSpell(mouseDirections, fizzledProjectile);
        }

        playerController.animator.SetBool("Casting", true);
        yield return new WaitForSeconds(secondsToHold);
        playerController.animator.SetBool("Casting", false);
        disableMovement = false;
        canCast = true;
    }

    private void fizzlingSpell(Dictionary<string, float> mouseDirections, string fizzledProjectile)
    {
        string direction = Tools.instance.GetDirectionAsString(mouseDirections["AttackH"], mouseDirections["AttackV"]);
        switch (fizzledProjectile)
        {
            case "LightningBolt":
                GameObject bolt;
                switch (direction)
                {
                    case "Up":
                        bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position + Vector3.up, Quaternion.identity);
                        bolt.GetComponent<LightningProjectile>().target = bolt.transform;
                        bolt.GetComponent<LightningProjectile>().DestroyAfterAnim("Up");
                        break;

                    case "Right":
                        bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position + Vector3.right, Quaternion.identity);
                        bolt.GetComponent<LightningProjectile>().target = bolt.transform;
                        bolt.GetComponent<LightningProjectile>().DestroyAfterAnim("Right");
                        break;

                    case "Down":
                        bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position + Vector3.down, Quaternion.identity);
                        bolt.GetComponent<LightningProjectile>().target = bolt.transform;
                        bolt.GetComponent<LightningProjectile>().DestroyAfterAnim("Down");
                        break;

                    case "Left":
                        bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position + Vector3.left, Quaternion.identity);
                        bolt.GetComponent<LightningProjectile>().target = bolt.transform;
                        bolt.GetComponent<LightningProjectile>().DestroyAfterAnim("Left");
                        break;
                }
                break;
        }
    }
    private void StopCoroutineIfNotNull(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
