using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CameraShake;

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

    private float lastH = 0.0f;
    private float lastV = 0.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 10f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private AudioClip dashClip;

    private bool canDash = true;
    private bool disableInput = false;
    private TrailRenderer dashTrail;


    // Keybinds
    [HideInInspector] public KeyCode meleeKey = KeyCode.Mouse0;
    [HideInInspector] public KeyCode dashKey = KeyCode.Mouse1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        potionsAndAbilities = GetComponent<PotionsAndAbilities>();
        dashTrail = GetComponent<TrailRenderer>();

        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        health = gm.playerHealth;

        healthText.text = "Health: " + health;
    }

  
    void Update()
    {
        if (disableInput)
            return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            lastH = Input.GetAxisRaw("Horizontal");
            lastV = Input.GetAxisRaw("Vertical");

            animator.SetFloat("lastHorizontal", lastH);
            animator.SetFloat("lastVertical", lastV);
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(meleeKey)) {
                if (PauseMenu.GameIsPaused)
                    return;

                Melee();
                nextAttackTime = Time.time + 1 / attackSpeed;
            }
        }
        if (Input.GetKeyDown(dashKey) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (disableInput)
            return;

        rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime);
    }

    // Collision handling
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
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
        if (disableInput)
            return;

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
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private IEnumerator Dash()
    {
        animator.SetTrigger("Dash");
        SoundManager.instance.PlaySound(dashClip);

        disableInput = true;
        canDash = false;
        rb.velocity = new Vector2(movement.normalized.x * dashingPower, movement.normalized.y * dashingPower);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        disableInput = false;
        dashTrail.emitting = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}
