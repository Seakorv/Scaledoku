using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject popUpMenu;
    [SerializeField] private List<Button> sudokus;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    private int sudokusLength;
    private int currentSudokuIndex = 0;

    [Header("SFX")]
    [SerializeField] private AK.Wwise.State sudokuOneState;
    [SerializeField] private AK.Wwise.State sudokuTwoState;
    [SerializeField] private AK.Wwise.State muteState;
    private MainMenuStates menuState;

    void Awake()
    {
        sudokusLength = sudokus.Count;
    }

    void Start()
    {
        sudokuOneState.SetValue();
        menuState = MainMenuStates.SudokuOne;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenSettings();
    }

    public void LoadSudoku(int sudokuSceneIndex)
    {
        ChangeMenuState(MainMenuStates.Mute);
        AkSoundEngine.StopAll();
        currentSudokuIndex = 0;
        SceneManager.LoadSceneAsync(sudokuSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextSudoku()
    {
        if (currentSudokuIndex <= 0)
        {
            leftButton.gameObject.SetActive(true);
        }

        ChangeMenuState(menuState + 1);
        sudokus[currentSudokuIndex++].gameObject.SetActive(false);
        sudokus[currentSudokuIndex].gameObject.SetActive(true);

        if (currentSudokuIndex >= sudokusLength - 1)
        {
            rightButton.gameObject.SetActive(false);
        }
    }

    public void PreviousSudoku()
    {
        if (currentSudokuIndex >= sudokusLength - 1)
        {
            rightButton.gameObject.SetActive(true);
        }

        ChangeMenuState(menuState - 1);
        sudokus[currentSudokuIndex--].gameObject.SetActive(false);
        sudokus[currentSudokuIndex].gameObject.SetActive(true);

        if (currentSudokuIndex <= 0)
        {
            leftButton.gameObject.SetActive(false);
        }
    }

    private void ChangeMenuState(MainMenuStates state)
    {
        switch (state)
        {
            case MainMenuStates.Mute:
                muteState.SetValue();
                break;
            case MainMenuStates.SudokuOne:
                sudokuOneState.SetValue();
                break;
            case MainMenuStates.SudokuTwo:
                sudokuTwoState.SetValue();
                break;
        }
        menuState = state;
    }

    public void OpenSettings()
    {
        popUpMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        popUpMenu.SetActive(false);
    }
}

public enum MainMenuStates
{
    Mute,
    SudokuOne,
    SudokuTwo,
}
