using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public CorridorFirstGenerator boardScript;
    public SimpleRandomWalkGenerator bossRoomScript;
    public bool isBossLevel = false;

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
    public string currentPlayerCharacterString;

    public float saveStopwatch = -1f;

    public InputActionAsset actions;

    /* ~~~~~~~~~~~ DEV ~~~~~~~~~~~ */
    public bool ForceBossRoomNext = true;
    private bool UseCurrentLevel = true;
    /* ~~~~~~~~~~~ DEV ~~~~~~~~~~~ */

    void Awake()
    {
        if (instance == null) 
            instance = this;
        else if (instance != this) 
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
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
        if (!IsBossLevelChecker() && !UseCurrentLevel)
        {
            boardScript.tilemapVisualizer = GameObject.FindGameObjectWithTag("TilemapVisualizer").GetComponent<TilemapVisualizer>();
            boardScript.RunProceduralGeneration(/*level*/);
        }
        else if (!ForceBossRoomNext && !UseCurrentLevel)
        {
            /*
            bossRoomScript.tilemapVisualizer = GameObject.FindGameObjectWithTag("TilemapVisualizer").GetComponent<TilemapVisualizer>();
            bossRoomScript.RunProceduralGeneration();
            */

            SceneManager.LoadScene(2);
        }

        Time.timeScale = 1f;

        gameOverImage = GameObject.Find("GameOverImage");
        gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        
        gameOverImage.SetActive(false);

        levelImage = GameObject.FindGameObjectWithTag("LevelImage");
        levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);

        //UpgradePanelObject = GameObject.FindGameObjectWithTag("UpgradePanel");
        StartCoroutine(HideLevelImage());
        
        
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScore(500);

        scoreUpdateText = GameObject.FindGameObjectWithTag("ScoreTextUpdate").GetComponent<TextMeshProUGUI>();
        //healthUpdateText = GameObject.FindGameObjectWithTag("HealthTextUpdate").GetComponent<TextMeshProUGUI>();

        // logic dealing with when to show upgrades can be found in IEnumerator HideLevelImage().
    }

    IEnumerator HideLevelImage()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(levelStartDelay);
        levelImage.SetActive(false);

        /*
        if (level % 4 == 0) {
            ShowUpgradePanelAfterLevelImageHidden();
        }
        else
            StopWatch.stopwatchActive = true;
        */
        StopWatch.stopwatchActive = true;
    }

    private void ShowUpgradePanelAfterLevelImageHidden() {
        //yield return new WaitForSeconds(levelStartDelay);
        Time.timeScale = 0f;
        UpgradePanelObject.SetActive(true);
    }

    public void GameOver() {
        StopWatch.stopwatchActive = false;

        Time.timeScale = 0f;
        SoundManager.instance.PauseGameplaySfx();

        gameOverText.text = "You Died at level " + level + "!";
        gameOverImage.SetActive(true);
    }

    public void setPlayerForNextLevel(int health, GameObject playerCharacter)
    {
        playerHealth = health;
        currentPlayerCharacterString = playerCharacter.name;
    }
    public GameObject GetCurrentPlayer()
    {
        if (currentPlayerCharacterString.Equals(GameAssets.i.WASDCharacter.name) || currentPlayerCharacterString.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
            return GameAssets.i.WASDCharacter;
        else if (currentPlayerCharacterString.Equals(GameAssets.i.MOBACharacter.name) || currentPlayerCharacterString.Equals(GameAssets.i.MOBACharacter.name + "(Clone)"))
            return GameAssets.i.MOBACharacter;
        else
            return GameAssets.i.MOBACharacter; //defaultPlayer
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
            //healthUpdateText.text = input;
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

    private bool IsBossLevelChecker()
    {
        if (level % 5 == 0 || ForceBossRoomNext)
        {
            isBossLevel = true;
            return true;
        }

        isBossLevel = false;
        return false;
    }
}
