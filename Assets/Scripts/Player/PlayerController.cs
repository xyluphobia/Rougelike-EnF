using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using CameraShake;
using System;
using UnityEngine.Rendering.Universal;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool activePlayer = false;

    public int health;
    public int maxHealth = 100;

    [HideInInspector] public Animator animator;
    private Rigidbody2D rb;
    private HealthBar healthBarScript;
    private GameManager gm;

    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip[] takeDamageClips;

    public float movementSpeed = 4.0f;
    public float restartLevelDelay = 1f;
    
    [HideInInspector] public bool disableInput = false;
    [HideInInspector] public bool invulnerable = false;
    public Light2D playerGlow;
    
    private PlayerInput playerInput;

    public Sprite portraitToInput;
    private Image playerPortrait;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
        if (activePlayer)
            setActivePlayer();

        healthBarScript = GameObject.Find("HealthBar").GetComponent<HealthBar>();
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        health = gm.playerHealth;
    }

    public void setActivePlayer()
    {
        gameObject.tag = "Player";
        playerInput.enabled = true;

        playerPortrait = GameObject.FindGameObjectWithTag("PortraitImage").GetComponent<Image>();
        playerPortrait.sprite = portraitToInput;
    }

    

    /* Inputs */
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


    /* Collisions */
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
    }

    /* Damage & Healing */
    public void HealDamage(int healBy)
    {
        health += healBy;

        if (health > maxHealth)
            health = maxHealth;

        healthBarScript.setHealth(health, maxHealth);
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

        health -= damage;
        healthBarScript.setHealth(health, maxHealth);

        animator.SetTrigger("Hurt");
        
        if (health <= 0)
            Die();
    }

    void Die() 
    {
        foreach (var item in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            item.BroadcastMessage("OnPlayerDied", SendMessageOptions.DontRequireReceiver);
        }

        SoundManager.instance.RandomizeSfx(deathClips);
        animator.SetBool("IsDead", true);
        GameManager.instance.GameOver();
    }

    /* Other */
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
}