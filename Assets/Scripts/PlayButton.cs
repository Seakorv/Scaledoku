using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    [SerializeField] private SpriteRenderer myArrow;
    public bool PlaySelected { get; private set; } = false;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        if (myArrow != null)
        {
            myArrow.color = startColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        HighlightPlayButton(true);
        PlaySelected = true;
    }

    void OnMouseExit()
    {
        HighlightPlayButton(false);
        PlaySelected = false;
    }

    public void HighlightPlayButton(bool highlight)
    {
        if (highlight) 
        {
            spriteRenderer.color = new Color(255, 255, 255);
            if (myArrow != null) myArrow.color = new Color(255, 255, 255);
        }
        else 
        {
            spriteRenderer.color = startColor;
            if (myArrow != null) myArrow.color = startColor;
        }
    }

    public void SetPlaySelected(bool amISelected)
    {
        PlaySelected = amISelected;
    }
}
