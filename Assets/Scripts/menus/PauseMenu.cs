using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    /*
    Code for restart and entering the main menu are located on the 'GameOver' script attatched to
    the GameOverImage.
    */


    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;



    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (GameIsPaused && OptionsMenu.isInOptionsMenu == false)  
                ResumeGame();
            else 
                PauseGame();
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void RestartGame() 
    {
        GameManager.instance.RestartGame();
    }

    public void QuitGame()
    {
        Debug.Log("quiting game");
        Application.Quit();
    }
}
