using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScaleSelector : MonoBehaviour
{
    public static ScaleSelector scaleSelectorInstance;
    private ScaleSelectorSquare[] squares;
    [Tooltip("Correct scale as numbers. Start from 1, its the 'prime'.")]
    [SerializeField] int[] correctScale;
    [SerializeField] string scale;
    [SerializeField] string[] chromaticScale;
    [SerializeField] Text scaleText;
    public string Scale { get { return scale; } }
    public bool ImCompleted { get; private set; } = false;
    private int proposedNoteAmount;
    private bool checkedIfCorrect = false;
    // Start is called before the first frame update
    void Awake()
    {
        scaleSelectorInstance = this;
    }

    void Start()
    {
        squares = FindObjectsOfType<ScaleSelectorSquare>();
        squares = BubbleSort(squares);
        for (int i = 0; i < squares.Length; i++)
        {
            squares[i].SetNoteText(chromaticScale[i]);
            Debug.Log(squares[i].Note + " Square Note");
        }
    }

    private ScaleSelectorSquare[] BubbleSort(ScaleSelectorSquare[] unsortedSquares)
    {
        ScaleSelectorSquare helpSquare;
        bool swapped;

        for (int i = 0; i < unsortedSquares.Length -1; i++)
        {
            swapped = false;
            for (int j = 0; j < unsortedSquares.Length -i - 1; j++)
            {
                if (unsortedSquares[j].Note > unsortedSquares[j+1].Note)
                {
                    helpSquare = unsortedSquares[j];
                    unsortedSquares[j] = unsortedSquares[j+1];
                    unsortedSquares[j+1] = helpSquare;
                    swapped = true;
                }
            }
            if (!swapped) break;
        }

        return unsortedSquares;
    }

    // Update is called once per frame
    void Update()
    {
        if (proposedNoteAmount == correctScale.Length)
        {
            if (!checkedIfCorrect)
            {
                if (CheckIfCorrect() && !ImCompleted)
                {
                    ImCompleted = true;
                    ScaleCompleted();
                    Debug.Log("Jippii");
                }
                Debug.Log("Väärin");
            }
        }
    }

    public void IncOrDecProposedNotes(bool incOrDec)
    {
        if (checkedIfCorrect) checkedIfCorrect = false;
        if (incOrDec) proposedNoteAmount++;
        else proposedNoteAmount--;
        Debug.Log(proposedNoteAmount + " = " + correctScale.Length);
    }

    private bool CheckIfCorrect()
    {
        checkedIfCorrect = true;
        int[] chosenNotes = new int[correctScale.Length];
        int chosenIndex = 0;
        for (int i = 0; i < squares.Length; i++)
        {
            if (squares[i].Selected)
            {
                chosenNotes[chosenIndex++] = squares[i].Note;
            }
        }
        for(int i = 0; i < correctScale.Length;i++)
        {
            Debug.Log(" Correct Scale: " + correctScale[i]);
        }

        for(int i = 0; i < correctScale.Length;i++)
        {
            Debug.Log(" Chosen : " + chosenNotes[i]);
        }

        for (int i = 0; i < correctScale.Length; i++)
        {
            if (correctScale[i] != chosenNotes[i]) return false;
        }
        return true;
    }

    private void ScaleCompleted()
    {
        scaleText.text = scale;
        for (int i = 0; i < squares.Length; i++)
        {
            if (!squares[i].Selected)
            {
                squares[i].gameObject.SetActive(false);
            }
        }
        StartCoroutine(SudokuManager.sudokuInstance.GenerateSudoku());
    }
}
