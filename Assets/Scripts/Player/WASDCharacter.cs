using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WASDCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerController playerController;
    private Image[] AbilityBarImageHolder;
    private Dictionary<string, Image> AbilityImagesDict = new();

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [HideInInspector] public float dashCooldownReduction = 0f;
    [SerializeField] private GameObject afterImage;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private Collider2D triggerCollider;
    private bool ghostingActive = false;

    public bool canDash = true; /* should be private outside of development*/

    [Header("Attack Settings")]
    [SerializeField] private AudioClip[] meleeClips;
    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointDown;
    [SerializeField] private Transform attackPointLeft;
    [SerializeField] private Transform midReference;
    private Transform attackPoint;

    public float attackRange = 0.8f;
    public int attackDamage = 30;
    public float attackSpeed = 2.0f;
    private float nextAttackTime = 0.0f;
    [SerializeField] private LayerMask enemyLayers;

    private PlayerInput playerInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        AbilityBarImageHolder = GameAssets.i.AbilityBarWASD.GetComponentsInChildren<Image>(true);
        foreach (Image img in AbilityBarImageHolder)
        {
            AbilityImagesDict.Add(img.transform.name, img);
            img.fillAmount = 0f;
        };
    }

    void FixedUpdate()
    {
        if (playerController.disableInput) return;

        rb.MovePosition(rb.position + movement.normalized * playerController.movementSpeed * Time.fixedDeltaTime);
    }

    public void TakeControl()
    {
        GetComponent<PlayerController>().SetActiveAbilityBar(GameAssets.i.AbilityBarWASD);
    }

    /* Inputs */
    private void OnMove(InputValue inputValue)
    {
        movement.x = inputValue.Get<Vector2>().x;
        movement.y = inputValue.Get<Vector2>().y;


        playerController.animator.SetFloat("Horizontal", movement.x);
        playerController.animator.SetFloat("Vertical", movement.y);
        playerController.animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x == 1 || movement.x == -1 || movement.y == 1 || movement.y == -1)
        {
            playerController.animator.SetFloat("lastHorizontal", movement.x);
            playerController.animator.SetFloat("lastVertical", movement.y);
        }
    }

    private void OnMelee()
    {
        if (!playerController.disableInput && Time.time >= nextAttackTime)
        {
            Melee();
            nextAttackTime = Time.time + 1.0f;
        }
    }

    private void OnDash()
    {
        if (!playerController.disableInput && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    /* Abilities */
    public void Melee(bool firedByAi = false)
    {
        Vector2 directionFromCast;

        if (firedByAi)
        {
            directionFromCast = (Tools.instance.FindClosestObjectByTag(transform.position).transform.position - midReference.transform.position).normalized;
        }
        else
        {
            Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePositionWorld.z = midReference.transform.position.z;

            Vector3 mousePositionPlayer = mousePositionWorld - midReference.transform.position;
            directionFromCast = mousePositionPlayer.normalized;
        }

        SoundManager.instance.RandomizeSfx(meleeClips);

        if (directionFromCast.x > 0.7)
        {
            attackPoint = attackPointRight;
        }
        else if (directionFromCast.x < -0.7)
        {
            attackPoint = attackPointLeft;
        }
        else if (directionFromCast.y > 0.7)
        {
            attackPoint = attackPointUp;
        }
        else if (directionFromCast.y < -0.7)
        {
            attackPoint = attackPointDown;
        }

        playerController.animator.SetFloat("attackH", directionFromCast.x);
        playerController.animator.SetFloat("attackV", directionFromCast.y);

        playerController.animator.SetTrigger("Melee");

        // Collects all hit units into an array  |  Gives center point of attackPoint's position, radius of attackRange and will hit anything on enemyLayers
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("PassDamage"))
            {
                enemy.transform.parent.GetComponentInChildren<Enemy>().TakeDamage(attackDamage);
            }
            else
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
        }
    }

    private IEnumerator Dash()
    {
        playerController.animator.SetTrigger("Dash");
        SoundManager.instance.PlaySound(dashClip);
        canDash = false;
        AbilityImagesDict["DashTimer"].fillAmount = 1f;

        playerController.invulnerable = true;
        StartCoroutine(ToggleGhosting(true));
        playerController.disableInput = true;

        rb.velocity = new Vector2(movement.normalized.x * dashingPower, movement.normalized.y * dashingPower);

        yield return new WaitForSeconds(dashDuration);

        playerController.disableInput = false;
        StartCoroutine(ToggleGhosting(false, true, 0.25f));
        playerController.invulnerable = false;
        dashCooldown -= dashCooldownReduction;
        dashCooldownReduction = 0f;

        StartCoroutine(playerController.CooldownUIUpdater(AbilityImagesDict["DashTimer"], dashCooldown, true));
        yield return new WaitForSeconds(Math.Max(0, dashCooldown));

        canDash = true;
    }

    public IEnumerator ToggleGhosting(bool activate = false, bool linger = false, float lingerDuration = 0f)
    {
        if (!ghostingActive || activate)
        {
            ghostingActive = true;
            StartCoroutine(UpdateColorGradually(new Color32(167, 220, 235, 230), 6f));
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(157, 220, 245, 200);

            Physics2D.IgnoreLayerCollision(6, 9, true);
            triggerCollider.enabled = true;

            StartCoroutine(AfterImage());
        }
        else
        {
            if (linger)
            {
                yield return new WaitForSeconds(lingerDuration);
            }

            StartCoroutine(UpdateColorGradually(new Color32(255, 235, 143, 255), 6f));
            ghostingActive = false;
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

            Physics2D.IgnoreLayerCollision(6, 9, false);
            triggerCollider.enabled = false;
        }

        IEnumerator UpdateColorGradually(Color goalColor, float speed)
        {
            Color startColor = playerController.playerGlow.color;

            float tick = 0f;
            while (playerController.playerGlow.color != goalColor)
            {
                tick += Time.deltaTime * speed;
                playerController.playerGlow.color = Color.Lerp(startColor, goalColor, tick);
                yield return null;
            }
        }

        IEnumerator AfterImage()
        {
            afterImage.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            int clonesSpawned = 0;

            while (clonesSpawned < 3)
            {
                clonesSpawned++;
                Instantiate(afterImage, gameObject.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.07f);
            }
        }
    }
}
