using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Tooltip("Which note of the scale it is. Treat it like a sudoku number. Zero is none, empty tile.")]
    [SerializeField] private int note = 0;
    public int Note { get { return note; } }

    [SerializeField] private SpriteRenderer notZero;
    [SerializeField] private SpriteRenderer zero;
    [SerializeField] private SpriteRenderer notZeroHL;
    [SerializeField] private SpriteRenderer zeroHL;
    [SerializeField] private SpriteRenderer redBox;
    [SerializeField] private SpriteRenderer presetColor;

    public string Name { get; set; }
    public int BoxID { get; set; } = 0;
    public int TileID { get; set; } = 0;
    private bool isPreset = false;

    // Raycast for touch input system
    

    // Start is called before the first frame update
    void Awake()
    {
        notZero.gameObject.SetActive(false);
        notZeroHL.gameObject.SetActive(false);
        redBox.gameObject.SetActive(false);
        presetColor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeNote(int newNote)
    {
        note = newNote;
        if (note != 0 && !isPreset) 
        {
            notZero.gameObject.SetActive(true);
            zeroHL.gameObject.SetActive(false);
            zero.gameObject.SetActive(false);
        }
        if (isPreset)
        {
            zeroHL.gameObject.SetActive(false);
            zero.gameObject.SetActive(false);
        }
        if (note == 0) 
        {
            notZero.gameObject.SetActive(false);
            notZeroHL.gameObject.SetActive(false);
            zero.gameObject.SetActive(true);
        }
    }

    void OnMouseEnter()
    {   
        if (!SudokuManager.sudokuInstance.IsTilePressed && !SudokuManager.sudokuInstance.InputDisabled) 
        {
            Highlight(true);
        }
    }

    void OnMouseExit()
    {
        if (!SudokuManager.sudokuInstance.InputDisabled) Highlight(false);
    }

    public void OnTouch()
    {
        SudokuManager.sudokuInstance.PlayMyNote(note);
        if (!SudokuManager.sudokuInstance.InputDisabled && !isPreset)
        {
            SudokuManager.sudokuInstance.TilePressed(this);
            //Highlight(true);
        }
        if (!SudokuManager.sudokuInstance.InputDisabled && isPreset)
        {
            SudokuManager.sudokuInstance.PresetTilePressed(this);
        }
    }

    public void OnTouchRelease()
    {
        if (!isPreset)
        {
            SudokuManager.sudokuInstance.TileReleased(this);
            //Highlight(false);
        }
        else SudokuManager.sudokuInstance.PresetTileReleased(this);
    }

    public void Highlight(bool onEnter)
    {
        if (onEnter) 
        {
            if (note == 0) zeroHL.gameObject.SetActive(true);
            if (note != 0) notZeroHL.gameObject.SetActive(true);
        }
        if (!onEnter)
        {
            if (note == 0) zeroHL.gameObject.SetActive(false);
            if (note != 0) notZeroHL.gameObject.SetActive(false);
        }
    }

    public void ActivateRedBox(bool activate)
    {
        redBox.gameObject.SetActive(activate);
    }

    public void SetPreset()
    {
        presetColor.gameObject.SetActive(true);
        isPreset = true;
    }
}
