using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider thisSlider;

    [Header("Wwise things")]
    [SerializeField] AK.Wwise.RTPC volumeRTPC;


    void Start()
    {
        thisSlider.value = volumeRTPC.GetGlobalValue();
    }

    /// <summary>
    /// Setting the value and volume for a given slider.
    /// </summary>
    /// <param name="volume">0 = master, 1 = music, 2 = sfx</param>
    public void SetVolume()
    {
        volumeRTPC.SetGlobalValue(thisSlider.value);
    }
}
