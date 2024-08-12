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
        PlayerInput playerInput = GameManager.instance.playerReference.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("PauseUI");

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        GameManager.instance.GameStatus = GameManager.GameStatusEnum.Playing;
        PlayerInput playerInput = GameManager.instance.playerReference.GetComponent<PlayerInput>();

        string stringToCheck = GameManager.instance.currentPlayerCharacterString;
        if (stringToCheck.Equals(GameAssets.i.WASDCharacter.name) || stringToCheck.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
            playerInput.SwitchCurrentActionMap("Gameplay");
        else if (stringToCheck.Equals(GameAssets.i.MOBACharacter.name) || stringToCheck.Equals(GameAssets.i.MOBACharacter.name + "(Clone)"))
            playerInput.SwitchCurrentActionMap("GameplayMOBA");
        else
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
