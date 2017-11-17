using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {
    private float secondsElapsed = 0;
    public Text timerTxt;
    // Use this for initialization
    void Start () {
        InvokeRepeating("incrementTime", 0.01f, 0.01f);
    }
	
	void incrementTime()
    {
        secondsElapsed += 0.01f;
        TimeSpan t = TimeSpan.FromSeconds(secondsElapsed);
        timerTxt.text = string.Format("{0:D2}:{1:D2}:{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
    }

    public string getTimeString()
    {
        TimeSpan t = TimeSpan.FromSeconds(secondsElapsed);
        return string.Format("{0:D2}:{1:D2}:{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
    }
}
