using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using UnityEngine;

public class SelectorCircle : MonoBehaviour
{
    [Header("All the 'buttons' of the selector circle.")]
    [SerializeField] private DeleteNumber deleteNote;
    [SerializeField] private PlayButton playButtonMe;
    [SerializeField] private PlayButton playButtonBox;
    [SerializeField] private PlayButton playButtonRow;
    [SerializeField] private PlayButton playButtonCol;
    [SerializeField] private CircleSector[] sectors;
    public static SelectorCircle selectorCircleInstance;
    // Start is called before the first frame update

    void Awake()
    {
        selectorCircleInstance = this;
    }
    void Start()
    {
        sectors = GameObject.FindObjectsOfType<CircleSector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentSectorNote(Tile tile)
    {
        if (deleteNote.IsActive()) return 0;
        if (playButtonMe.PlaySelected) return SudokuManager.sudokuInstance.CurrentTileNote;
        else if (playButtonBox.PlaySelected || playButtonRow.PlaySelected || playButtonCol.PlaySelected) 
        {
            if (playButtonBox.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(0, tile);
            else if (playButtonRow.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(1, tile);
            else if (playButtonCol.PlaySelected) SudokuManager.sudokuInstance.StartPlayScaleCoroutine(2, tile);
            return SudokuManager.sudokuInstance.CurrentTileNote;
        }
        for (int i = 0; i < sectors.Length; i++)
        {
            if (sectors[i].ImSelected)
            {
                return sectors[i].MyNote;
            }
        }
        return 0; // Deleting the note
    }

    public void RemoveHighlights()
    {
        deleteNote.SetSpriteActive(false);
        playButtonBox.HighlightPlayButton(false);
        playButtonBox.SetPlaySelected(false);

        playButtonRow.HighlightPlayButton(false);
        playButtonRow.SetPlaySelected(false);

        playButtonCol.HighlightPlayButton(false);
        playButtonCol.SetPlaySelected(false);
        for (int i = 0; i < sectors.Length; i++)
        {
            sectors[i].ImNotSelectedAnymore();
            sectors[i].HighlightSector(false);
        }
    }
}
