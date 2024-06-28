using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNumber : MonoBehaviour
{
    [SerializeField] private SpriteRenderer redCross;

    // Start is called before the first frame update
    void Start()
    {
        redCross.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        redCross.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        redCross.gameObject.SetActive(false);
    }

    void OnMouseUp()
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
