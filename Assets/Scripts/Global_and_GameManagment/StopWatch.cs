using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StopWatch : MonoBehaviour
{
    public static bool stopwatchActive = false;

    public float currentTime = 0f;
    public TextMeshProUGUI currentTimeText;


    void Start() {
        if (GameManager.instance.saveStopwatch > -1)
            currentTime = GameManager.instance.saveStopwatch;
    }

    void FixedUpdate()
    {
        if (stopwatchActive) {
            currentTime = currentTime + Time.fixedDeltaTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = time.ToString(@"mm\:ss\:ff");
    }

    void OnDestroy() {
        GameManager.instance.saveStopwatch = currentTime;
        stopwatchActive = false;
    }
}
