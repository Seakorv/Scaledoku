using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SudokuManager : MonoBehaviour
{
    [SerializeField] private float tempo = 141;
    [SerializeField] private float generationSpeed = 0.02f;
    private bool sudokuCompleted = false;
    private float eigthNote;
    private float quarterNote;
    [Tooltip("Dimensions for the sudoku")]
    [SerializeField] private int sudokuHeight, sudokuWidth;
    [Tooltip("One box dimensions, 9x9 sudoku has 3x3 boxes.")]
    [SerializeField] private int boxHeight, boxWidth;
    [SerializeField] private float gridPadding = 0.07f;
    public int BoxSize { get; private set;}
    [SerializeField] private Tile tilefab;
    private Tile[,] tiles;
    [SerializeField] private Transform sudokuCamera;
    [SerializeField] private Transform selectorCircle;
    [SerializeField] private Transform presetPlay;
    public static SudokuManager sudokuInstance;
    public bool IsTilePressed { get; private set; } = false;
    public bool InputDisabled { get; private set; } = false;
    private Tile currentTile;
    public int CurrentTileNote { get; private set; }

    [Tooltip("Write the sudoku numbers as a continuous string. Left to right and from bottom, 0 is empty.")]
    [SerializeField] string sudokuNumbers;
    
    [Header("UI things")]
    [SerializeField] private Text progress;
    private int percentage = 0;
    private int howManyZeros = 0;
    private int maxZeros = 0;

    [Header("SFX")]
    [SerializeField] private AK.Wwise.Event primeSFX;
    [SerializeField] private AK.Wwise.Event secondSFX;
    [SerializeField] private AK.Wwise.Event thirdSFX;
    [SerializeField] private AK.Wwise.Event fourthSFX;
    [SerializeField] private AK.Wwise.Event fifthSFX;
    [SerializeField] private AK.Wwise.Event sixthSFX;
    [SerializeField] private AK.Wwise.Event seventhSFX;
    [SerializeField] private AK.Wwise.Event octaveSFX;
    [SerializeField] private AK.Wwise.Event ninthSFX;
    [SerializeField] private AK.Wwise.Event errorSFX;

    [Header("Sound States")]
    [SerializeField] private AK.Wwise.State introState;
    [SerializeField] private AK.Wwise.State firstProg;
    [SerializeField] private AK.Wwise.State secondProg;
    [SerializeField] private AK.Wwise.State thirdProg;
    [SerializeField] private AK.Wwise.State fourthProg;
    [SerializeField] private AK.Wwise.State fifthProg;
    [SerializeField] private AK.Wwise.State sixthProg;
    [SerializeField] private AK.Wwise.State seventhProg;
    [SerializeField] private AK.Wwise.State outroState;
    [SerializeField] private AK.Wwise.State succesState;
    public BGMusicState bgState;
    [SerializeField] private bool sudokuWithIntro = false;

    [Tooltip("Percentage, when this progress will be active. if 10, 0-9% will be this.")]
    [SerializeField] private int progress0 = 10;
    [SerializeField] private int progress1 = 20;
    [SerializeField] private int progress2 = 30;
    [SerializeField] private int progress3 = 40;
    [SerializeField] private int progress4 = 50;
    [SerializeField] private int progress5 = 69;
    [SerializeField] private int progress6 = 85;
    [SerializeField] private int progress7 = 100;
    
    
    void Awake()
    {
        quarterNote = 60f / tempo;
        eigthNote = quarterNote / 2f;
        BoxSize = boxHeight * boxWidth;
        tiles = new Tile[sudokuWidth, sudokuHeight];
        sudokuInstance = this;
        progress.text = percentage + "%";
    }

    // Start is called before the first frame update
    void Start()
    {
        selectorCircle.gameObject.SetActive(false);
        presetPlay.gameObject.SetActive(false);
        introState.SetValue();
        bgState = BGMusicState.intro;
        //GenerateSudoku();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public IEnumerator GenerateSudoku()
    {
        if (sudokuWithIntro)
        {
            firstProg.SetValue();
            bgState = BGMusicState.firstProg;
        }
        sudokuCamera.transform.position = new Vector3((float)sudokuWidth / 2, (float)sudokuHeight / 2, -10);
        ScaleSelector.scaleSelectorInstance.transform.position = new Vector3((float)sudokuWidth / 2, -1.25f);
        int sudokuStringIndex = 0;
        int currentTileID = 0;
        // First generate the sudoku
        int counterY = 0;
        float boxPaddingY = 0f;
        float paddingX = 0.5f;
        float paddingY = 0.5f;
        for (int y = 0; y < sudokuHeight; y++)
        {
            int counterX = 0;
            float boxPaddingX = 0f;

            if (counterY == boxHeight)
            {
                boxPaddingY += gridPadding;
                //paddingX += boxPaddingY;
                counterY = 0;
            }
            counterY++;

            for (int x = 0; x < sudokuWidth; x++)
            {
                if (counterX == boxWidth )
                {
                    boxPaddingX += gridPadding;
                    //paddingX += boxPaddingX;
                    counterX = 0;
                }
                counterX++;
                Tile spawnedTile = Instantiate(tilefab, new Vector3((float)x + paddingX + boxPaddingX, (float)y + paddingY + boxPaddingY), Quaternion.identity);
                spawnedTile.TileID = currentTileID++;
                spawnedTile.Highlight(false);
                int presetNumber = ToInt(sudokuNumbers[sudokuStringIndex++]);
                if (presetNumber != 0) spawnedTile.SetPreset();
                else howManyZeros++;
                spawnedTile.ChangeNote(presetNumber);
                tiles[x,y] = spawnedTile;
                //boxPaddingX = 0f;
                yield return new WaitForSeconds(generationSpeed);
            }
            //boxPaddingY = 0f;
        }

        // Then go through the sudoku and give every tile information 
        // which box they belong to.
        if (boxHeight != 0 || boxWidth != 0) // Just in case
        {
            int boxStartX = 0;
            int boxStartY = 0;
            int currentBoxID = 0;
            for (int y = 0; y < sudokuHeight / boxHeight; y++)
            {
                for (int x = 0; x < sudokuWidth / boxWidth; x++)
                {
                    for (int by = boxStartY; by < boxStartY + boxHeight; by++)
                    {
                        for (int bx = boxStartX; bx < boxStartX + boxWidth; bx++)
                        {
                            tiles[bx,by].BoxID = currentBoxID;
                        }
                    }
                    currentBoxID++;
                    boxStartX += boxWidth;
                }
                boxStartX = 0;
                boxStartY += boxHeight;
            }
        }
        maxZeros = howManyZeros;
    }

    public void TilePressed(Tile tile)
    {
        IsTilePressed = true;
        selectorCircle.gameObject.SetActive(true);
        selectorCircle.position = tile.transform.position;
        currentTile = tile;
        CurrentTileNote = tile.Note;
    }

    public void TileReleased(Tile tile)
    {
        bool cancel = false;
        IsTilePressed = false;
        int proposedNote = SelectorCircle.selectorCircleInstance.GetCurrentSectorNote(tile);
        if (!SelectorCircle.selectorCircleInstance.DeleteNote.DeleteSelected && proposedNote == 0) cancel = true;

        if (proposedNote != tile.Note && !cancel)
        {
            bool legal = CheckIfLegal(tile, proposedNote);
            if (!legal) errorSFX.Post(gameObject);
            if (legal) 
            {
                if (proposedNote != 0) howManyZeros--;
                else howManyZeros++;
                UpdateProgress();
                tile.ChangeNote(proposedNote);
                bool setCompleted = false;
                string isSetCompleted = CheckIfSetCompleted(tile);
                if (isSetCompleted != "NO") setCompleted = true;
                if (setCompleted && !sudokuCompleted) StartCoroutine(SuccesState(isSetCompleted, tile));
            }
        }
        else if (tile.Note != 0)
        {
            //TODO: maybe something
        }
        SelectorCircle.selectorCircleInstance.RemoveHighlights();
        selectorCircle.gameObject.SetActive(false);
    }

    public void PresetTilePressed(Tile tile)
    {
        IsTilePressed = true;
        presetPlay.gameObject.SetActive(true);
        presetPlay.position = tile.transform.position;
    }

    public void PresetTileReleased(Tile tile)
    {
        IsTilePressed = false;
        PresetPlayManager.presetPlayInstance.WhichPresetPlay(tile);
        PresetPlayManager.presetPlayInstance.RemovePresetHighlights();
        presetPlay.gameObject.SetActive(false);
    }


    /// <summary>
    /// Checking if the proposed note (=number) is legal according to sudoku rules.
    /// Always checking first if it is not OK so we can quit early.
    /// First we'll find the index of the tile and also checking if the tile's box has the number.
    /// Then we'll check if the row has the number and finally the column.
    /// </summary>
    /// <param name="tile">The tile which note will be changed</param>
    /// <param name="proposedNote">The incoming note</param>
    /// <returns></returns>
    private bool CheckIfLegal(Tile tile, int proposedNote)
    {
        Debug.Log("Incoming note: " + proposedNote);
        // If the number is 0, we can always say true.
        if (proposedNote == 0) return true;

        int myIndexX = 0;
        int myIndexY = 0;
        // Finding the index of the tile in the tiles matrix.
        // Checking also if the tile's box has the same number
        for (int y = 0; y < sudokuHeight; y++)
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                // If they are from the same box and note matches
                if (tiles[x, y].BoxID == tile.BoxID && tiles[x, y].Note == proposedNote) 
                {
                    StartCoroutine(ShowError("BOX", tile.BoxID));
                    return false; 
                }
                if (tiles[x, y] == tile) 
                {
                    myIndexX = x;
                    myIndexY = y;
                }
            }
        }

        // Checking if the row has the number
        for (int x = 0; x < sudokuWidth; x++)
        {
            if (tiles[x, myIndexY].Note == proposedNote) 
            {
                StartCoroutine(ShowError("ROW", myIndexY));
                return false;
            }
        }

        //Checking if the column has the number
        for (int y = 0; y < sudokuHeight; y++)
        {
            if (tiles[myIndexX, y].Note == proposedNote) 
            {
                StartCoroutine(ShowError("COL", myIndexX));
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Shows red and plays error sound when trying illegal numbers
    /// </summary>
    /// <param name="whereExit">Where did the error first occur.
    ///                         Use only "BOX", "ROW", or "COL"</param>
    /// <param name="indexOrBoxID">Box needs the boxID, Row needs the y-index of the tile,
    ///                         Col needs the x-index.</param>
    /// <returns></returns>
    private IEnumerator ShowError(string whereExit, int indexOrBoxID)
    {
        InputDisabled = true;
        if (whereExit == "BOX")
        {
            for (int y = 0; y < sudokuHeight; y++)
            {
                for (int x = 0; x < sudokuWidth; x++)
                {
                    if (tiles[x, y].BoxID == indexOrBoxID) tiles[x, y].ActivateRedBox(true);
                }
            }
            yield return new WaitForSeconds(quarterNote);
            for (int y = 0; y < sudokuHeight; y++)
            {
                for (int x = 0; x < sudokuWidth; x++)
                {
                    if (tiles[x, y].BoxID == indexOrBoxID) tiles[x, y].ActivateRedBox(false);
                }
            }
        }
        else if (whereExit == "ROW")
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                tiles[x, indexOrBoxID].ActivateRedBox(true);
            }
            yield return new WaitForSeconds(quarterNote);
            for (int x = 0; x < sudokuWidth; x++)
            {
                tiles[x, indexOrBoxID].ActivateRedBox(false);
            }
        }
        else if (whereExit == "COL")
        {
            for (int y = 0; y < sudokuHeight; y++)
            {
                tiles[indexOrBoxID, y].ActivateRedBox(true);
            }
            yield return new WaitForSeconds(quarterNote);
            for (int y = 0; y < sudokuHeight; y++)
            {
                tiles[indexOrBoxID, y].ActivateRedBox(false);
            }
        }
        InputDisabled = false;
    }

    private IEnumerator SuccesState(string whichWay, Tile tile)
    {
        InputDisabled = true;
        SetBGState(BGMusicState.succes);
        
        // Getting tile's index AGAIN, maybe I should do a method for this 
        int myIndexX = 0;
        int myIndexY = 0;

        for (int y = 0; y < sudokuHeight; y++)
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                if (tiles[x, y] == tile) 
                {
                    myIndexX = x;
                    myIndexY = y;
                }
            }
        }

        if (whichWay == "BOX")
        {
            for (int y = 0; y < sudokuHeight; y++)
            {
                for (int x = 0; x < sudokuWidth; x++)
                {
                    if (tiles[x, y].BoxID == tile.BoxID) tiles[x, y].Highlight(true);
                }
            }
            yield return new WaitForSeconds(quarterNote * 3);
            for (int y = 0; y < sudokuHeight; y++)
            {
                for (int x = 0; x < sudokuWidth; x++)
                {
                    if (tiles[x, y].BoxID == tile.BoxID) tiles[x, y].Highlight(false);
                }
            }
        }

        else if (whichWay == "ROW")
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                tiles[x, myIndexY].Highlight(true);
            }
            yield return new WaitForSeconds(quarterNote * 3);
            for (int x = 0; x < sudokuWidth; x++)
            {
                tiles[x, myIndexY].Highlight(false);
            }
        }

        else if (whichWay == "COL")
        {
            for (int y = 0; y < sudokuHeight; y++)
            {
                tiles[myIndexX, y].Highlight(true);
            }
            yield return new WaitForSeconds(quarterNote * 3);
            for (int y = 0; y < sudokuHeight; y++)
            {
                tiles[myIndexX, y].Highlight(false);
            }
        }

        InputDisabled = false;
        SetBGState(bgState);
    }


    /// <summary>
    /// Method to start coroutine from this script because SelectorCircle will be inactive while they should play
    /// </summary>
    /// <param name="playWhichWay">0 is box, 1 is row, 2 is col</param>
    public void StartPlayScaleCoroutine(int playWhichWay, Tile tile)
    {
        StartCoroutine(PlayScale(playWhichWay, tile));
    }


    /// <summary>
    /// Play notes of chosen box/row/column
    /// </summary>
    /// <param name="whichWay">which set will be played, 0 is box, 1 is row, 2 is col</param>
    /// <returns></returns>
    public IEnumerator PlayScale(int whichWay, Tile tile)
    {
        InputDisabled = true;
        int myIndexY = 0;
        int myIndexX = 0;

        // Putting the tiles in scale order here. Length is always sudoku height.
        // Will be sorted later
        Tile[] playThese = new Tile[sudokuHeight];

        // Getting the tile index again, like in CheckIfLegal()
        for (int y = 0; y < sudokuHeight; y++)
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                if (tiles[x, y] == tile) 
                {
                    myIndexX = x;
                    myIndexY = y;
                }
            }
        }

        int tileIndex = 0;

        // Getting the correct tiles in the sorting array
        switch (whichWay) 
        {
            case 0:
                for (int y = 0; y < sudokuHeight; y++)
                {
                    for (int x = 0; x < sudokuWidth; x++)
                    {
                        if (tiles[x, y].BoxID == tile.BoxID)
                        {
                            playThese[tileIndex++] = tiles[x, y];
                        }
                    }
                }
                break;
            case 1:
                for (int x = 0; x < sudokuWidth; x++)
                {
                    playThese[tileIndex++] = tiles[x, myIndexY];
                }
                break;
            case 2:
                for (int y = 0; y < sudokuHeight; y++)
                {
                    playThese[tileIndex++] = tiles[myIndexX, y];
                }
                break;
        }

        // Sorting tiles
        playThese = BubbleSort(playThese);

        // Playing the notes and highlighting tiles
        for (int i = 0; i < playThese.Length; i++)
        {
            if (playThese[i].Note == 0) continue;
            PlayMyNote(playThese[i].Note);
            playThese[i].Highlight(true);
            yield return new WaitForSeconds(eigthNote); 
            playThese[i].Highlight(false);
        }

        InputDisabled = false;
    }


    /// <summary>
    /// Bubble sorting the tiles from scale prime to last
    /// </summary>
    /// <param name="tiles">tiles to be sorted</param>
    /// <returns>sorted tiles from 1->x</returns>
    private Tile[] BubbleSort(Tile[] tiles)
    {
        Tile helpTile;
        bool swapped;

        for (int i = 0; i < tiles.Length -1; i++)
        {
            swapped = false;
            for (int j = 0; j < tiles.Length -i - 1; j++)
            {
                if (tiles[j].Note > tiles[j+1].Note)
                {
                    helpTile = tiles[j];
                    tiles[j] = tiles[j+1];
                    tiles[j+1] = helpTile;
                    swapped = true;
                }
            }
            if (!swapped) break;
        }

        return tiles;
    }


    /// <summary>
    /// Checking if a set is completed and returning if its a box, row or col
    /// </summary>
    /// <param name="tile">Tile which triggerst checking</param>
    /// <returns>"BOX", "ROW", or "COL". Not complete returns "NO"</returns>
    private string CheckIfSetCompleted(Tile tile)
    {
        // Getting tile's index AGAIN, maybe I should do a method for this 

        int myIndexX = 0;
        int myIndexY = 0;

        for (int y = 0; y < sudokuHeight; y++)
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                if (tiles[x, y] == tile) 
                {
                    myIndexX = x;
                    myIndexY = y;
                }
            }
        }
        bool boxComplete = true;
        // Checking if box is completed
        for (int y = 0; y < sudokuHeight; y++)
        {
            for (int x = 0; x < sudokuWidth; x++)
            {
                if (tiles[x,y].BoxID == tile.BoxID)
                {
                    if (tiles[x,y].Note == 0) 
                    {
                        boxComplete = false;
                        break;
                    }
                }
            }
        }
        if (boxComplete) return "BOX";

        // Checking the row
        bool rowComplete = true;
        for (int x = 0; x < sudokuWidth; x++)
        {
            if (tiles[x, myIndexY].Note == 0)
            {
                rowComplete = false;
                break;
            }
        }
        if (rowComplete) return "ROW";

        // Checking the column
        bool colComplete = true;
        for (int y = 0; y < sudokuHeight; y++)
        {
            if (tiles[myIndexX, y].Note == 0)
            {
                colComplete = false;
                break;
            }
        }
        if (colComplete) return "COL";

        // If we get here, none were completed
        return "NO";
    }


    public int ToInt(char c)
    {
        return (int)(c - '0');
    }

    private void UpdateProgress()
    {
        float percentageFloat = 100 - ((float)howManyZeros / (float)maxZeros * 100);
        percentage = (int)percentageFloat;

        // if sudoku has an intro music, which will be only active before generating
        // the sudoku, we do not want intro state changing here.
        if (percentage < progress0 && !sudokuWithIntro) SetBGState(BGMusicState.intro);
        else if (percentage < progress1) SetBGState(BGMusicState.firstProg);
        else if (percentage < progress2) SetBGState(BGMusicState.secondProg);
        else if (percentage < progress3) SetBGState(BGMusicState.thirdProg);
        else if (percentage < progress4) SetBGState(BGMusicState.fourthProg);
        else if (percentage < progress5) SetBGState(BGMusicState.fifthProg);
        else if (percentage < progress6) SetBGState(BGMusicState.sixthProg);
        else if (percentage < progress7) SetBGState(BGMusicState.seventhProg);
        else if (percentage >= 100) SetBGState(BGMusicState.outro);

        progress.text = percentage + "%";
    }

    private void SetBGState(BGMusicState state)
    {
        switch (state) 
        {
            case BGMusicState.intro:
                introState.SetValue();
                break;
            case BGMusicState.firstProg:
                firstProg.SetValue();
                break;
            case BGMusicState.secondProg:
                secondProg.SetValue();
                break;
            case BGMusicState.thirdProg:
                thirdProg.SetValue();
                break;
            case BGMusicState.fourthProg:
                fourthProg.SetValue();
                break;
            case BGMusicState.fifthProg:
                fifthProg.SetValue();
                break;
            case BGMusicState.sixthProg:
                sixthProg.SetValue();
                break;
            case BGMusicState.seventhProg:
                seventhProg.SetValue();
                break;
            case BGMusicState.outro:
                outroState.SetValue();
                sudokuCompleted = true;
                InputDisabled = true;
                StartCoroutine(CloseSudoku());
                break;
            case BGMusicState.succes:
                succesState.SetValue();
                break;
            
        }
        if (state != BGMusicState.succes) bgState = state;
    }

    /// <summary>
    /// Plays the given note by interval. In the sudokumanager so the
    /// scale is easier to manage.
    /// </summary>
    /// <param name="tileNote">Which note will be played</param>
    public void PlayMyNote(int tileNote)
    {
        switch (tileNote)
        {
            case 0: 
                break;
            case 1:
                primeSFX.Post(gameObject);
                break;
            case 2:
                secondSFX.Post(gameObject);
                break;
            case 3:
                thirdSFX.Post(gameObject);
                break;
            case 4:
                fourthSFX.Post(gameObject);
                break;
            case 5:
                fifthSFX.Post(gameObject);
                break;
            case 6:
                sixthSFX.Post(gameObject);
                break;
            case 7:
                seventhSFX.Post(gameObject);
                break;
            case 8:
                octaveSFX.Post(gameObject);
                break;
            case 9:
                ninthSFX.Post(gameObject);
                break;
        }
    }


    private IEnumerator CloseSudoku()
    {
        int x = 0;
        int y = 0;
        int index = 0;
        int rounds = 0;
        tiles[x, y].gameObject.SetActive(false);
        yield return new WaitForSeconds(generationSpeed);

        while (index < tiles.Length - 1)
        {
            for (int i = y + 1; i < sudokuHeight - rounds; i++)
            {
                tiles[x, i].gameObject.SetActive(false);
                y++;
                index++;
                yield return new WaitForSeconds(generationSpeed);
            }
            Debug.Log("Y: " + y + " X: " + x);
            for (int i = x + 1; i < sudokuWidth - rounds; i++)
            {
                tiles[i, y].gameObject.SetActive(false);
                x++;
                index++;
                yield return new WaitForSeconds(generationSpeed);
            }
            Debug.Log("Y: " + y + " X: " + x);
            for (int i = y - 1; i >= rounds; i--)
            {
                tiles[x, i].gameObject.SetActive(false);
                y--;
                index++;
                yield return new WaitForSeconds(generationSpeed);
            }
            Debug.Log("Y: " + y + " X: " + x);
            rounds++;
            for (int i = x - 1; x > rounds; i--)
            {
                tiles[i, y].gameObject.SetActive(false);
                x--;
                index++;
                yield return new WaitForSeconds(generationSpeed);
            }
        }
        Debug.Log(index + " Index");
        Debug.Log(tiles.Length + " Tiles Length");
        Debug.Log(rounds + " Rounds");
    }
}

public enum BGMusicState
{
    intro,
    firstProg,
    secondProg,
    thirdProg,
    fourthProg,
    fifthProg,
    sixthProg,
    seventhProg,
    outro,
    succes,
}

