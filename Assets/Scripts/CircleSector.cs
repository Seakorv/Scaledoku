using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSector : MonoBehaviour
{
    [SerializeField] int myNote;
    public int MyNote
    {
        get { return myNote; }
    }
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    public bool ImSelected {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        HighlightSector(true);
        ImSelected = true;
        SudokuManager.sudokuInstance.PlayMyNote(myNote);
    }

    void OnMouseExit()
    {
        HighlightSector(false);
        ImSelected = false;
    }

    public void HighlightSector(bool highlight)
    {
        if (highlight) spriteRenderer.color = new Color(255, 255, 255);
        else spriteRenderer.color = startColor;
    }

    public void ImNotSelectedAnymore()
    {
        ImSelected = false;
    }
}
