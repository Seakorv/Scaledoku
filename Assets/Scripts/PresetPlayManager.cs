using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetPlayManager : MonoBehaviour
{
    public static PresetPlayManager presetPlayInstance;
    [SerializeField] private PlayButton boxPlay;
    [SerializeField] private PlayButton rowPlay;
    [SerializeField] private PlayButton colPlay;
    // Start is called before the first frame update
    void Awake()
    {
        presetPlayInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhichPresetPlay(Tile tile)
    {
        if (boxPlay.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(0, tile);
        else if (rowPlay.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(1, tile);
        else if (colPlay.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(2, tile);
    }

    public void RemovePresetHighlights()
    {
        boxPlay.HighlightPlayButton(false);
        boxPlay.SetPlaySelected(false);

        rowPlay.HighlightPlayButton(false);
        rowPlay.SetPlaySelected(false);

        colPlay.HighlightPlayButton(false);
        colPlay.SetPlaySelected(false);
    }
}
