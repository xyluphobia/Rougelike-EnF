using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public CorridorFirstGenerator boardScript;

    public float levelStartDelay = 1.4f;
    public int level = 0;


    private TextMeshProUGUI levelText;
    private GameObject levelImage;
    private TextMeshProUGUI gameOverText;
    private GameObject gameOverImage;

    public TextMeshProUGUI scoreText;
    public int score = -500;

    private TextMeshProUGUI scoreUpdateText;
    private TextMeshProUGUI healthUpdateText;

    private GameObject UpgradePanelObject;

    public int playerHealth = 100;

    void Awake()
    {
        if (instance == null) 
            instance = this;
        else if (instance != this) 
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<CorridorFirstGenerator>();
    }

    private void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        Time.timeScale = 1f;

        gameOverImage = GameObject.Find("GameOverImage");
        gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        
        gameOverImage.SetActive(false);

        levelImage = GameObject.FindGameObjectWithTag("LevelImage");
        levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);


        StartCoroutine(HideLevelImage());
        
        boardScript.tilemapVisualizer = GameObject.FindGameObjectWithTag("TilemapVisualizer").GetComponent<TilemapVisualizer>();
        boardScript.RunProceduralGeneration(/*level*/);
        
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScore(500);

        scoreUpdateText = GameObject.FindGameObjectWithTag("ScoreTextUpdate").GetComponent<TextMeshProUGUI>();
        healthUpdateText = GameObject.FindGameObjectWithTag("HealthTextUpdate").GetComponent<TextMeshProUGUI>();


        if (level % 4 == 0)
        {
            UpgradePanelObject = GameObject.FindGameObjectWithTag("UpgradePanel");
            StartCoroutine(ShowUpgradePanelAfterLevelImageHidden());
        }
    }

    IEnumerator HideLevelImage()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(levelStartDelay);
        levelImage.SetActive(false);
    }

    IEnumerator ShowUpgradePanelAfterLevelImageHidden() {
        yield return new WaitForSeconds(levelStartDelay);
        Time.timeScale = 0f;

        UpgradePanelObject.SetActive(true);
    }

    public void GameOver() {
        Time.timeScale = 0f;
        SoundManager.instance.PauseGameplaySfx();

        gameOverText.text = "You Died at level " + level + "!";
        gameOverImage.SetActive(true);
    }

    public void setPlayerHealth(int health) {
        playerHealth = health;
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = $"{score:000000}";

        TextChangeVisualizer(true, "+" + scoreToAdd.ToString());
    }

    public void TextChangeVisualizer(bool isScoreUpdate, string input)
    {
        if (!scoreUpdateText && !healthUpdateText)
            return;

        if (isScoreUpdate)
        {
            scoreUpdateText.text = input;
            scoreUpdateText.CrossFadeAlpha(1.0f, 0.0f, false);
            StartCoroutine(fadeTextOverTime(scoreUpdateText));
        }
        else
        {
            healthUpdateText.text = input;
            if (input[0] == '-')
                healthUpdateText.color = new Color32(176, 0, 137, 255);
            else 
                healthUpdateText.color = new Color32(176, 0, 28, 255);
            healthUpdateText.CrossFadeAlpha(1.0f, 0.0f, false);
            StartCoroutine(fadeTextOverTime(healthUpdateText));
        }
    }
    IEnumerator fadeTextOverTime(TextMeshProUGUI textObject)
    {
        yield return new WaitForSeconds(0.5f);
        textObject.CrossFadeAlpha(0.0f, 0.5f, false);
    }


    public void ResetBeforePlay()
    {
        level = 0;
        score = -500;
        playerHealth = 100;
    }

    public void RestartGame()
    {
        level = 0;
        score = -500;
        playerHealth = 100;

        SoundManager.instance.ResumeGameplaySfx();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ShowText(GameObject textObject, int input, GameObject self)
    {
        if (textObject)
        {   
            GameObject prefab = Instantiate(textObject, self.transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMeshPro>().text = input.ToString();
        }
    }
}
