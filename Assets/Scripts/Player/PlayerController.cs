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
    private float defaultMovementSpeed;
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

        healthBarScript = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        defaultMovementSpeed = movementSpeed;
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        health = gm.playerHealth;

        if (activePlayer)
            setActivePlayer();
        else
            setInactivePlayer();
    }

    public void setActivePlayer()
    {
        activePlayer = true;

        gameObject.tag = "Player";
        playerInput.enabled = true;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SetCameraTarget(gameObject);

        playerPortrait = GameObject.FindGameObjectWithTag("PortraitImage").GetComponent<Image>();
        playerPortrait.sprite = portraitToInput;

        gameObject.SendMessage("TakeControl", null, SendMessageOptions.DontRequireReceiver);
        
    }
    private void setInactivePlayer()
    {
        activePlayer = false;

        gameObject.tag = "Interactable";
        playerInput.enabled = false;

        gameObject.SendMessage("GiveControl", null, SendMessageOptions.DontRequireReceiver);
    }
    
    public void SetActiveAbilityBar(GameObject BarToSet)
    {
        if (GameAssets.i.AbilityBarActive != null)
        {
            GameAssets.i.AbilityBarActive.SetActive(false);
        }
        GameAssets.i.AbilityBarActive = BarToSet;
        BarToSet.SetActive(true);
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

    private void OnInteract()
    {
        RaycastHit2D[] nearbyObjects = Physics2D.CircleCastAll(transform.position, 2f, (Vector2)transform.position);

        foreach (RaycastHit2D item in nearbyObjects)
        {
            if (item.transform.gameObject.CompareTag("Interactable"))
            {
                GameObject interactableObject = item.transform.gameObject;

                if (interactableObject.layer == 6)  // Layer 6 is the player layer.
                {
                    setInactivePlayer();
                    interactableObject.GetComponent<PlayerController>().setActivePlayer();
                    break;
                }
            }
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

            GameManager.instance.setPlayerForNextLevel(health, gameObject);

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
        if (movementSpeed > defaultMovementSpeed)
            return;
        
        movementSpeed = movementSpeed * buff;
        animator.speed *= buff;
    }

    public void ResetBoost()
    {
        movementSpeed = defaultMovementSpeed;
        animator.speed = 1;
    }

    public IEnumerator CooldownUIUpdater(Image TimerUI, float Cooldown, bool ImageFillSet = false)
    {
        if (!ImageFillSet)
            TimerUI.fillAmount = 1;

        while (TimerUI.fillAmount > 0)
        {
            TimerUI.fillAmount -= 1 / Cooldown * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        TimerUI.fillAmount = 0;
    }
}