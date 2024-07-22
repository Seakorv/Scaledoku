using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleSelectorSquare : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Color highlight;

    [SerializeField] private Text noteText;
    [SerializeField] private int note;
    private AK.Wwise.Event noteSFX;
    public int Note
    {
        get { return note; }
    }
    public bool Selected { get; private set; } = false;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        highlight = new Color(255, 255, 255);
    }

    void Start()
    {
        switch (note)
        {
            case 1:
                noteSFX = ScaleSelector.scaleSelectorInstance.note1;
                break;
            case 2:
                noteSFX = ScaleSelector.scaleSelectorInstance.note2;
                break;
            case 3:
                noteSFX = ScaleSelector.scaleSelectorInstance.note3;
                break;
            case 4:
                noteSFX = ScaleSelector.scaleSelectorInstance.note4;
                break;
            case 5:
                noteSFX = ScaleSelector.scaleSelectorInstance.note5;
                break;
            case 6:
                noteSFX = ScaleSelector.scaleSelectorInstance.note6;
                break;
            case 7:
                noteSFX = ScaleSelector.scaleSelectorInstance.note7;
                break;
            case 8:
                noteSFX = ScaleSelector.scaleSelectorInstance.note8;
                break;
            case 9:
                noteSFX = ScaleSelector.scaleSelectorInstance.note9;
                break;
            case 10:
                noteSFX = ScaleSelector.scaleSelectorInstance.note10;
                break;
            case 11:
                noteSFX = ScaleSelector.scaleSelectorInstance.note11;
                break;
            case 12:
                noteSFX = ScaleSelector.scaleSelectorInstance.note12;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTouch()
    {
        if (!Selected) 
        {
            spriteRenderer.color  = highlight;
            noteSFX.Post(gameObject);
            Selected = true;
            ScaleSelector.scaleSelectorInstance.IncOrDecProposedNotes(true);
        }
        else if (!ScaleSelector.scaleSelectorInstance.ImCompleted)
        {
            noteSFX.Post(gameObject);
            Selected = false;
            ScaleSelector.scaleSelectorInstance.IncOrDecProposedNotes(false);
        }
    }

    public void OnTouchExit()
    {
        if (!Selected && !ScaleSelector.scaleSelectorInstance.ImCompleted) spriteRenderer.color = startColor;
    }

    public void SetNoteText(string noteAsTxt)
    {
        noteText.text = noteAsTxt;
    }
}
