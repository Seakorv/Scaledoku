using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUpMenu : MonoBehaviour
{
    [SerializeField] GameObject soundSettings;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    private PopUpMenu menu;

    void Awake()
    {
        menu = this;
    }

    public void ToMainMenu()
    {
        SudokuManager.sudokuInstance.SetMuteState();
        AkSoundEngine.StopAll();
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit(9);
    }

    public void SoundSettingsActive()
    {
        soundSettings.SetActive(true);
    }


    public void CloseSoundSettings()
    {
        soundSettings.SetActive(false);
    }

    public void CloseMenu()
    {
        SudokuManager.sudokuInstance.CloseMenu();
    }
}
