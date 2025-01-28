using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class AspectRatioKeeper : MonoBehaviour
{
    [SerializeField] private float aspectX;
    [SerializeField] private float aspectY;

    void Start()
    {
        ScaleAspect();
    }


    private void ScaleAspect()
    {
        float targetAspect = aspectX / aspectY;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Camera camera = Camera.main;

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            
            camera.rect = rect;
        }
        else
        {
            float scalWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scalWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
