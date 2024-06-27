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

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [HideInInspector] public float dashCooldownReduction = 0f;
    [SerializeField] private AudioClip dashClip;
    private Image dashTimerUI;

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

    // Keybinds
    private PlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        dashTimerUI = GameObject.Find("DashTimer").GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if (playerController.disableInput) return;

        rb.MovePosition(rb.position + movement.normalized * playerController.movementSpeed * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue inputValue)
    {
        if (playerController.disableInput) return;

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

    void Melee()
    {
        SoundManager.instance.RandomizeSfx(meleeClips);

        Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionWorld.z = midReference.transform.position.z;

        Vector3 mousePositionPlayer = mousePositionWorld - midReference.transform.position;
        mousePositionPlayer.Normalize();

        if (mousePositionPlayer.x > 0.7)
        {
            attackPoint = attackPointRight;
        }
        else if (mousePositionPlayer.x < -0.7)
        {
            attackPoint = attackPointLeft;
        }
        else if (mousePositionPlayer.y > 0.7)
        {
            attackPoint = attackPointUp;
        }
        else if (mousePositionPlayer.y < -0.7)
        {
            attackPoint = attackPointDown;
        }

        // Debug.Log(mousePositionPlayer);
        playerController.animator.SetFloat("attackH", mousePositionPlayer.x);
        playerController.animator.SetFloat("attackV", mousePositionPlayer.y);

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
        dashTimerUI.fillAmount = 1f;

        playerController.invulnerable = true;
        StartCoroutine(playerController.ToggleGhosting(true));
        playerController.disableInput = true;

        rb.velocity = new Vector2(movement.normalized.x * dashingPower, movement.normalized.y * dashingPower);

        yield return new WaitForSeconds(dashDuration);

        playerController.disableInput = false;
        StartCoroutine(playerController.ToggleGhosting(false, true, 0.25f));
        playerController.invulnerable = false;
        dashCooldown -= dashCooldownReduction;
        dashCooldownReduction = 0f;

        StartCoroutine(DashCooldownUIUpdates());
        yield return new WaitForSeconds(Math.Max(0, dashCooldown));

        canDash = true;

        IEnumerator DashCooldownUIUpdates()
        {
            while (dashTimerUI.fillAmount > 0)
            {
                dashTimerUI.fillAmount -= 1 / dashCooldown * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            dashTimerUI.fillAmount = 0;
        }
    }
}
