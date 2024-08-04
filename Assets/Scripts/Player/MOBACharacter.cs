using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MOBACharacter : MonoBehaviour
{
    private PlayerController playerController;
    private NavMeshAgent navAgent;

    private GameObject movementIndicatorArrow;
    private Image[] AbilityBarImageHolder;
    private Dictionary<string, Image> AbilityImagesDict = new();
    [HideInInspector] public bool disableMovement = false;
    [HideInInspector] public bool canCast = true;
    [SerializeField] private AudioClip fizzledCastSfx;

    [SerializeField] private AudioClip lightningCastSfx;
    private bool abilityOneIsOnCooldown = false;
    [HideInInspector] public float abilityOneCooldown = 1.5f;

    [SerializeField] private AudioClip icicleCastSfx;
    private bool abilityTwoIsOnCooldownRapid = false;
    [HideInInspector] public float abilityTwoCooldownRapid = 0.5f;
    [HideInInspector] public float abilityTwoCooldownGeneral = 3.5f;
    private int icicleRapidShotsUsed = 0;
    private Coroutine activeUITimer = null;
    private Coroutine activeGeneralIcicleTimer = null;
    [HideInInspector] public Dictionary<GameObject, int> enemyFreezeCounters = new();

    [SerializeField] private AudioClip teleportStartSfx;
    [SerializeField] private AudioClip teleportEndSfx;
    private bool abilityThreeIsOnCooldown = false;
    [HideInInspector] public float abilityThreeCooldown = 5f;

    public RadialMenuMB characterSelectMenuPrefab;
    protected RadialMenuMB characterSelectMenuInstance;

    [HideInInspector] public GameObject selectedCharacter;
    [SerializeField] private AudioClip ultimateSfx;
    private bool ultimateIsOnCooldown = false;
    private float ultimateCooldown = 3f;
    private bool ultimateSwapPositionIsOnCooldown = false;
    private float ultimateSwapPositionCooldown = 5f;
    private Coroutine swapTimer;
    [HideInInspector] public GameObject currentClone;

    private Vector2 clickedPos;
    private Vector2 lastFramePos;
    private Vector2 currentFramePos;

    [HideInInspector] public bool firedByAi = false;

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
            img.fillAmount = 0f;
        }

        StartRTimerCoroutine();
    }

    private void FixedUpdate() // could find direction by saving previous step and comparing against current position then checking which direction was travelled
    {
        currentFramePos = transform.position;

        if (currentFramePos != lastFramePos)
        {
            Vector2 diffVector = new(System.Math.Abs(currentFramePos.x - lastFramePos.x), System.Math.Abs(currentFramePos.y - lastFramePos.y));

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

            if (diffXPercent > 0.7f || diffXPercent < -0.7f || diffYPercent > 0.7f || diffYPercent < -0.7f)
            {
                playerController.animator.SetFloat("lastHorizontal", diffXPercent);
                playerController.animator.SetFloat("lastVertical", diffYPercent);
            }
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

        GetComponent<PlayerController>().SetActiveAbilityBar(GameAssets.i.AbilityBarMOBA);
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
    public void OnAbilityOne() /* |Q| Basic Damage Ability ~ Lightning Bolt */
    {
        if (abilityOneIsOnCooldown || !canCast) return;

        Vector3 abilityOrigin;
        float circleCastSize;
        if (firedByAi)
        {
            abilityOrigin = transform.position;
            circleCastSize = 15f;
        }
        else
        {
            abilityOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            circleCastSize = 4f;
            StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["QTimer"], abilityOneCooldown));
        }

        canCast = false;

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        Vector2 mousePos = abilityOrigin;

        RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(mousePos, circleCastSize, (Vector2)transform.position);
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
            StartCoroutine(HoldCastPositionForSeconds(0.25f, closestEnemy.position));
            StartCoroutine(LightningAbilityCooldown());
            SoundManager.instance.PlaySound(lightningCastSfx);

            GameObject bolt = Instantiate(GameAssets.i.lightningBoltProjectile, transform.position, Quaternion.identity);
            bolt.GetComponent<LightningProjectile>().spawnedByPlayer = true;
            bolt.GetComponent<LightningProjectile>().target = closestEnemy;
        }
        else if (!firedByAi)
        {
            StartCoroutine(HoldCastPositionForSeconds(0.25f, abilityOrigin, true, "LightningBolt"));
            SoundManager.instance.PlaySound(fizzledCastSfx);

            canCast = true;
        }

        IEnumerator LightningAbilityCooldown()
        {
            abilityOneIsOnCooldown = true;
            yield return new WaitForSeconds(abilityOneCooldown);
            abilityOneIsOnCooldown = false;
        }
    }

    public void OnAbilityTwo() /* |W| Interesting Ability ~ Icicle */
    {
        // Target is slowed stacking with each hit until 3 hits where the target is frozen completely
        // Fires towards mouse NO TRACKING
        // Allows 3 activations using the rapid cooldown amount, after these activations no more casts are allowed until the general cooldown is finished
        // The general cooldown starts after each of the 3 activations so if the timer elapses before 3 casts are used the ability is reset fully to 0 rapid fires used

        if (abilityTwoIsOnCooldownRapid || !canCast) return;

        Vector3 abilityOrigin;
        if (firedByAi)
        {
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;
            RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(transform.position, 15f, (Vector2)transform.position);
            foreach (RaycastHit2D enemy in nearbyEnemies)
            {
                if (enemy.transform.CompareTag("Enemy"))
                {
                    float distance = Vector3.Distance(enemy.point, transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.transform;
                    }
                }
            }

            if (closestEnemy != null)
            {
                abilityOrigin = closestEnemy.position;
            }
            else
            {
                return;
            }
        }
        else
        {
            abilityOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        canCast = false;
        if (icicleRapidShotsUsed < 3)
        {
            StopCoroutineIfNotNull(activeUITimer);
            StopCoroutineIfNotNull(activeGeneralIcicleTimer);

            activeGeneralIcicleTimer = StartCoroutine(IcicleGeneralAbilityCooldown());
            StartCoroutine(HoldCastPositionForSeconds(0.25f, abilityOrigin));

            GameObject icicle = Instantiate(GameAssets.i.icicleProjectile, transform.position, Quaternion.identity);
            if (icicleRapidShotsUsed != 2) /* First Two Shots ~ Slows 30% | 60% */
            {
                if (!firedByAi)
                    activeUITimer = StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["WTimer"], abilityTwoCooldownRapid));
                StartCoroutine(IcicleRapidAbilityCooldown());

                if (icicleRapidShotsUsed == 1)
                {
                    icicle.transform.localScale *= 1.15f;
                    icicle.GetComponent<IcicleProjectile>().projectileSpeed = 6f;
                }
            }
            else /* Third shot ~ Freezes 100% (No Movement & No Attacks) */
            {
                if (!firedByAi)
                    activeUITimer = StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["WTimer"], abilityTwoCooldownGeneral));
                icicle.transform.localScale *= 1.3f;
                icicle.GetComponent<IcicleProjectile>().projectileSpeed = 7f;
            }
            icicle.GetComponent<IcicleProjectile>().SetDirection(abilityOrigin);

            // spawn projectile
            // scale the projectile up based on which shot it is making the 3rd one largest
            icicleRapidShotsUsed += 1;
        }
        canCast = true;

        IEnumerator IcicleGeneralAbilityCooldown()
        {
            yield return new WaitForSeconds(abilityTwoCooldownGeneral);
            icicleRapidShotsUsed = 0;
        }
        IEnumerator IcicleRapidAbilityCooldown()
        {
            abilityTwoIsOnCooldownRapid = true;
            yield return new WaitForSeconds(abilityTwoCooldownRapid);
            abilityTwoIsOnCooldownRapid = false;
        }
    }

    public void OnAbilityThree() /* |E| Movement Ability ~ Teleport */
    {
        if (abilityThreeIsOnCooldown || !canCast) return;

        Vector3 abilityOrigin;
        if (firedByAi)
        {
            abilityOrigin = transform.position + (Vector3)(6f * Random.insideUnitCircle);
        }
        else
        {
            abilityOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            movementIndicatorArrow.transform.localScale = Vector3.zero;
            StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["ETimer"], abilityThreeCooldown));
        }

        canCast = false;
        StartCoroutine(TeleportAbilityCooldown());

        disableMovement = true;
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;

        Vector2 teleportLocation = abilityOrigin;
        StartCoroutine(TeleportingAnimationHandler());

        IEnumerator TeleportingAnimationHandler()
        {
            playerController.animator.SetBool("Teleporting", true);
            SoundManager.instance.PlaySound(teleportStartSfx);
            yield return new WaitForSeconds(0.65f);  // Anim time is: 0.417s

            navAgent.Warp(teleportLocation);

            playerController.animator.SetBool("Teleporting", false);
            SoundManager.instance.PlaySound(teleportEndSfx);
            yield return new WaitForSeconds(0.333f);
            disableMovement = false;
            canCast = true;
        }

        IEnumerator TeleportAbilityCooldown()
        {
            abilityThreeIsOnCooldown = true;
            yield return new WaitForSeconds(abilityThreeCooldown);
            abilityThreeIsOnCooldown = false;
        }
    }

    public void OnUltimateAbility() /* |R| Ultimate Ability */
    {
        if (!canCast) return;

        if (ultimateIsOnCooldown && !ultimateSwapPositionIsOnCooldown && currentClone != null)
        {
            StartCoroutine(UltimateSwapPositionCooldown());
            swapTimer = StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["RTimer"], ultimateSwapPositionCooldown));
            
            currentClone.GetComponent<MOBA_WildMagicClone>().SwapWithPlayer(transform.position);
            return;
        }
        else if (!ultimateIsOnCooldown)
        {
            selectedCharacter = null;
            StartCoroutine(RunMOBAUltimate());

            IEnumerator RunMOBAUltimate()
            {
                ultimateIsOnCooldown = true;
                Vector3 abilityOrigin;
                if (firedByAi)
                {
                    abilityOrigin = transform.position;
                }
                else
                {
                    abilityOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }

                /* Start ~ This code deals with the selection of the character to spawn, the while loop prevents progress until a choice is made. */
                Tools.instance.PauseGame();
                characterSelectMenuInstance = Instantiate(characterSelectMenuPrefab, GameObject.FindGameObjectWithTag("InGameUI").transform);
                characterSelectMenuInstance.callback = MenuClick;

                while (selectedCharacter == null)
                {
                    yield return null;
                }

                Tools.instance.UnPauseGame();
                /* End */

                if (NavMesh.SamplePosition(abilityOrigin, out NavMeshHit closestNavPosition, 100, -1))
                {
                    GameObject character = Instantiate(selectedCharacter, closestNavPosition.position, Quaternion.identity);

                    character.GetComponent<Rigidbody2D>().isKinematic = true;
                    character.GetComponent<Rigidbody2D>().simulated = false;
                    character.AddComponent<MOBA_WildMagicClone>();
                    character.GetComponent<SpriteRenderer>().color = new Color32(47, 255, 255, 137);
                    character.GetComponentInChildren<Light2D>().color = new Color32(143, 236, 255, 255);
                    if (character.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
                    {
                        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                        agent.avoidancePriority = 99;
                    }

                    currentClone = character;
                }
                else
                {
                    Debug.Log("No closest nav position.");
                }
            }
        }

        void MenuClick(string path)
        {
            var paths = path.Split('/');
            selectedCharacter = GameManager.instance.GetPlayerObjectByName(paths[1]);
        }

        IEnumerator UltimateSwapPositionCooldown()
        {
            ultimateSwapPositionIsOnCooldown = true;
            yield return new WaitForSeconds(ultimateSwapPositionCooldown);
            ultimateSwapPositionIsOnCooldown = false;
        }
    }

    public void StartRTimerCoroutine()
    {
        if (ultimateSwapPositionIsOnCooldown)
        {
            StopCoroutine(swapTimer);
        }

        StartCoroutine(UltimateAbilityCooldown());
        StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["RTimer"], ultimateCooldown));
    }

    IEnumerator UltimateAbilityCooldown()
    {
        ultimateIsOnCooldown = true;
        yield return new WaitForSeconds(ultimateCooldown);
        ultimateIsOnCooldown = false;
    }

    IEnumerator HoldCastPositionForSeconds(float secondsToHold, Vector3 abilityOrigin, bool fizzling = false, string fizzledProjectile = "")
    {
        disableMovement = true;

        Dictionary<string, float> mouseDirections = Tools.instance.FindDirectionOfMouseFromPlayer(abilityOrigin, transform);
        playerController.animator.SetFloat("attackH", mouseDirections["AttackH"]);
        playerController.animator.SetFloat("attackV", mouseDirections["AttackV"]);

        if (fizzling)
        {
            FizzlingSpell(mouseDirections, fizzledProjectile);
        }

        playerController.animator.SetBool("Casting", true);
        yield return new WaitForSeconds(secondsToHold);
        playerController.animator.SetBool("Casting", false);
        disableMovement = false;
        canCast = true;
    }

    private void FizzlingSpell(Dictionary<string, float> mouseDirections, string fizzledProjectile)
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
