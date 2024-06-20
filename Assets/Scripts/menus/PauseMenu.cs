using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("PauseUI");

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Gameplay");
        
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
