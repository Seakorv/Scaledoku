using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNumber : MonoBehaviour
{
    [SerializeField] private SpriteRenderer redCross;
    public bool DeleteSelected { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        redCross.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTouchEnter()
    {
        redCross.gameObject.SetActive(true);
        DeleteSelected = true;
    }

    public void OnTouchExit()
    {
        redCross.gameObject.SetActive(false);
        DeleteSelected = false;
    }

    public void OnTouchUp()
    {
        redCross.gameObject.SetActive(false);
    }

    public void SetSpriteActive(bool active)
    {
        redCross.gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return redCross.gameObject.activeSelf;
    }   
}
