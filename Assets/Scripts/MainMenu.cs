using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadSudoku(int sudokuSceneIndex)
    {
        SceneManager.LoadSceneAsync(sudokuSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
