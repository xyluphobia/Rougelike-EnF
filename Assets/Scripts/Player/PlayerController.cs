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
    public int health;
    public int maxHealth = 100;

    [HideInInspector] public Animator animator;
    private Rigidbody2D rb;
    private HealthBar healthBarScript;

    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip[] takeDamageClips;
    [SerializeField] private GameObject afterImage;

    public float movementSpeed = 4.0f;
    public float restartLevelDelay = 1f;

    private PotionsAndAbilities potionsAndAbilities;
    private GameManager gm;
    
    [HideInInspector] public bool disableInput = false;
    [HideInInspector] public bool invulnerable = false;
    private bool ghostingActive = false;
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private Light2D playerGlow;
    
    private PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        potionsAndAbilities = GetComponent<PotionsAndAbilities>();
        playerInput = GetComponent<PlayerInput>();

        healthBarScript = GameObject.Find("HealthBar").GetComponent<HealthBar>();
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        health = gm.playerHealth;
    }

    

/* Movement and Ability Input Handling */
    

    

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
            Color startColor = playerGlow.color;

            float tick = 0f;
            while(playerGlow.color != goalColor)
            {
                tick += Time.deltaTime * speed;
                playerGlow.color = Color.Lerp(startColor, goalColor, tick);
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

    public void Boost(float buff)
    {
        movementSpeed = movementSpeed * buff;
    }

    public void ResetBoost(float buff)
    {
        movementSpeed = movementSpeed / buff;
    }
}