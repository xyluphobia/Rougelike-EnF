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



    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
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
