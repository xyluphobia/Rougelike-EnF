using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using CameraShake;
using System;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;

    [HideInInspector] public TextMeshProUGUI healthText;

    public float movementSpeed = 4.0f;
    public float restartLevelDelay = 1f;

    private Animator animator;

    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip[] takeDamageClips;
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

    private PotionsAndAbilities potionsAndAbilities;
    private GameManager gm;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [HideInInspector] public float dashCooldownReduction = 0f;
    [SerializeField] private AudioClip dashClip;

    /*private < should be private outside of development*/ public bool canDash = true;
    private bool disableInput = false;
    [HideInInspector] public bool invulnerable = false;
    private bool ghostingActive = false;
    [SerializeField] private Collider2D triggerCollider;

    private TrailRenderer dashTrail;
    [SerializeField] private Light2D playerGlow;


    // Keybinds
    private PlayerInput playerInput;
    

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        potionsAndAbilities = GetComponent<PotionsAndAbilities>();
        dashTrail = GetComponent<TrailRenderer>();
        playerInput = GetComponent<PlayerInput>();

        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        health = gm.playerHealth;

        healthText.text = "Health: " + health;
    }

    void FixedUpdate()
    {
        if (disableInput) return;

        rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime);
    }

/* Movement and Ability Input Handling */
    private void OnMove(InputValue inputValue) {
        if (disableInput) return;
        
        movement.x = inputValue.Get<Vector2>().x;
        movement.y = inputValue.Get<Vector2>().y;


        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x == 1 || movement.x == -1 || movement.y == 1 ||  movement.y == -1)
        {
            animator.SetFloat("lastHorizontal", movement.x);
            animator.SetFloat("lastVertical", movement.y);
        }
    }

    private void OnMelee() {
        if (!disableInput && Time.time >= nextAttackTime)
        {
            Melee();
            nextAttackTime = Time.time + 1.0f;
        }
    }

    private void OnDash() {
        if(!disableInput && canDash) 
        {
            StartCoroutine(Dash());
        }
    }

    private void OnPause() {
        PauseMenu pauseMenu = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<PauseMenu>();
        pauseMenu.PauseGame();
    }

    private void OnResume()
    {
        if (!OptionsMenu.isInOptionsMenu)
        {
            PauseMenu pauseMenu = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<PauseMenu>();
            pauseMenu.ResumeGame();
        }
        else
        {
            GameObject.FindGameObjectWithTag("OptionsMenu").SetActive(false);
        }
    }
    
    private void OnTestShit()
    {
        
    }


/* Collision Handling */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            invulnerable = true;
            StopWatch.stopwatchActive = false;
            GameManager.instance.ShowText(GameAssets.i.scoreText, 500, gameObject);

            rb.velocity = Vector2.zero;
            disableInput = true;

            GameManager.instance.setPlayerHealth(health);

            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.CompareTag("HealthPotion"))
        {
            potionsAndAbilities.healthPotion.useHealthPotion(0.25f);
            other.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else if (other.CompareTag("SpeedPotion"))
        {
            potionsAndAbilities.speedPotion.useSpeedPotion();
            other.gameObject.transform.parent.gameObject.SetActive(false);
        } 

    }

    private void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        disableInput = false;
    }

    public void Boost(float buff)
    {
        movementSpeed = movementSpeed * buff;
    }

    public void ResetBoost(float buff) 
    {
        movementSpeed = movementSpeed / buff;
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable)
        {
            return;
        }

        CameraShaker.Presets.ShortShake2D(0.06f, 0.12f, 30, 6);

        GameManager.instance.ShowText(GameAssets.i.damageText, damage, gameObject);
        SoundManager.instance.RandomizeSfx(takeDamageClips);

        GameManager.instance.TextChangeVisualizer(false, "-" + damage.ToString());

        health -= damage;

        // Debug.Log("Health: " + health);
        animator.SetTrigger("Hurt");
        
        if (health <= 0)
            Die();

        healthText.text = "Health: " + health;
    }

    void Die() 
    {
        SoundManager.instance.RandomizeSfx(deathClips);
        animator.SetBool("IsDead", true);
        GameManager.instance.GameOver();
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
        animator.SetFloat("attackH", mousePositionPlayer.x);
        animator.SetFloat("attackV", mousePositionPlayer.y);

        animator.SetTrigger("Melee");

        // Collects all hit units into an array  |  Gives center point of attackPoint's position, radius of attackRange and will hit anything on enemyLayers
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies) 
        {
            if (enemy.CompareTag("PassDamage"))
            {
                enemy.GetComponentInChildren<Enemy>().TakeDamage(attackDamage);
            }
            else
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
        }
    }

    private IEnumerator Dash()
    {
        animator.SetTrigger("Dash");
        SoundManager.instance.PlaySound(dashClip);
        canDash = false;

        invulnerable = true;
        StartCoroutine(ToggleGhosting(true));
        dashTrail.emitting = true;
        disableInput = true;

        rb.velocity = new Vector2(movement.normalized.x * dashingPower, movement.normalized.y * dashingPower);

        yield return new WaitForSeconds(dashDuration);

        disableInput = false;
        StartCoroutine(ToggleGhosting(false, true, 0.25f));
        dashTrail.emitting = false;
        invulnerable = false;

        yield return new WaitForSeconds(Math.Max(0, dashCooldown - dashCooldownReduction));

        canDash = true;
    }

    private IEnumerator ToggleGhosting(bool activate = false, bool linger = false, float lingerDuration = 0f)
    {
        if (!ghostingActive || activate)
        {
            ghostingActive = true;
            //playerGlow.color = new Color32(157, 220, 245, 200);
            StartCoroutine(UpdateColorGradually(new Color32(167, 220, 235, 230), 6f));
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(157, 220, 245, 200);

            Physics2D.IgnoreLayerCollision(6, 9, true);
            triggerCollider.enabled = true;
        }
        else
        {
            if (linger)
            {
                yield return new WaitForSeconds(lingerDuration);
            }

            StartCoroutine(UpdateColorGradually(new Color32(255, 235, 143, 255), 6f));
            ghostingActive = false;
            //playerGlow.color = new Color32(255, 235, 143, 255);
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

            Physics2D.IgnoreLayerCollision(6, 9, false);
            triggerCollider.enabled = false;
        }

        IEnumerator UpdateColorGradually(Color goalColor, float speed)
        {
            Color startColor = playerGlow.color;

            float tick = 0f;
            while(playerGlow.color != goalColor)
            {
                tick += Time.deltaTime * speed;
                playerGlow.color = Color.Lerp(startColor, goalColor, tick);
                yield return null;
            }
        }
    }
}