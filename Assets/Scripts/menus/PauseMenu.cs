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
        if (GameManager.instance.GameStatus == GameManager.GameStatusEnum.Unpausable) return;

        GameManager.instance.GameStatus = GameManager.GameStatusEnum.Paused;
        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("PauseUI");

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        GameManager.instance.GameStatus = GameManager.GameStatusEnum.Playing;
        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Gameplay");  // this first needs to detect which action map is required based on character then switch to it
        
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
