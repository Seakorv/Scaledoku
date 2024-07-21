using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    private Collider2D previousHit;
    private Collider2D currentSector;
    private Collider2D previousSector;
    private Collider2D previousPlay;
    private Collider2D deleteNumberCollider;
    private Tile currentTile;
    private bool tilePressed = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPressAction = playerInput.actions["TouchPress"];
    }

    private void OnEnable()
    {
        touchPositionAction.performed += GetTouchedObject;
        touchPressAction.performed += TouchObject;
        touchPressAction.canceled += ReleaseTouch;
    }

    private void OnDisable()
    {
        touchPositionAction.performed -= GetTouchedObject;
        touchPressAction.performed -= TouchObject;
        touchPressAction.canceled -= ReleaseTouch;
    }


    void Update()
    {
        if (tilePressed)
        {
            CheckTouch(touchPositionAction.ReadValue<Vector2>());
        }
    }


    private void GetTouchedObject(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CheckTouch(touchPositionAction.ReadValue<Vector2>());
        }
    }


    private void CheckTouch(Vector2 pos)
    {
        Vector2 wp = Camera.main.ScreenToWorldPoint(pos);
        Collider2D hit = Physics2D.OverlapPoint(wp);
        if (hit && !tilePressed)
        {
            previousHit = hit;
        }
        if (tilePressed && hit && currentSector != hit)
        {
            // Sector handling
            currentSector = hit;
            if (currentSector.CompareTag("Sector"))
            {
                if (deleteNumberCollider) deleteNumberCollider.gameObject.GetComponent<DeleteNumber>().OnTouchExit();
                currentSector.gameObject.GetComponent<CircleSector>().OnTouch();
                if (previousSector && currentSector && previousSector != currentSector)
                {
                    previousSector.gameObject.GetComponent<CircleSector>().OnTouchExit();
                }
                previousSector = currentSector;
            }
            if (!currentSector.CompareTag("Sector") && previousSector && previousSector.CompareTag("Sector"))
            {
                previousSector.gameObject.GetComponent<CircleSector>().OnTouchExit();
            }

            // Play Button Handling
            if (currentSector.CompareTag("PlayButton"))
            {
                if (deleteNumberCollider) deleteNumberCollider.gameObject.GetComponent<DeleteNumber>().OnTouchExit();
                currentSector.gameObject.GetComponent<PlayButton>().OnTouch();
                if (previousPlay && currentSector && previousPlay != currentSector)
                {
                    previousPlay.gameObject.GetComponent<PlayButton>().OnTouchExit();
                }
                previousPlay = currentSector;
            }
            if (!currentSector.CompareTag("PlayButton") && previousPlay && previousPlay.CompareTag("PlayButton"))
            {
                previousPlay.gameObject.GetComponent<PlayButton>().OnTouchExit();
            }

            // Delete Number Handling
            if (currentSector.CompareTag("DeleteNumber"))
            {
                deleteNumberCollider = currentSector;
                currentSector.gameObject.GetComponent<DeleteNumber>().OnTouchEnter();
            }
        }
    }

    private void TouchObject(InputAction.CallbackContext context)
    {
        if (previousHit.CompareTag("ScaleSquare"))
        {
            previousHit.gameObject.GetComponent<ScaleSelectorSquare>().OnTouch();
        }
        if (previousHit.CompareTag("Tile"))
        {
            tilePressed = true;
            currentTile = previousHit.gameObject.GetComponent<Tile>();
            currentTile.OnTouch();
        }
        if (previousHit.CompareTag("Sector"))
        {
            
        }
    }

    private void ReleaseTouch(InputAction.CallbackContext context)
    {
        if (previousHit.CompareTag("ScaleSquare"))
        {
            previousHit.gameObject.GetComponent<ScaleSelectorSquare>().OnTouchExit();
        }
        if (previousHit.CompareTag("Tile"))
        {
            tilePressed = false;
            currentTile.OnTouchRelease();
        }
    }
}
