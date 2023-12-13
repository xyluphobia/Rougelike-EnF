using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public void ReturnToMainMenu ()
    {
        SceneManager.LoadScene("MainMenu");
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
