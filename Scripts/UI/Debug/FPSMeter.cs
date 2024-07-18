using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSMeter : MonoBehaviour
{
    public float fps;
    public TextMeshProUGUI fpsMeterText;

    private void Start()
    {
        InvokeRepeating("GetFPS", 0.3f, 0.3f);
    }

    public void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsMeterText.text = $"FPS: {fps}";
    }
}
