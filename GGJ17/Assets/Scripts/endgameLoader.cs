using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class endgameLoader : MonoBehaviour {

    public Text timeText;
    public Text deathText;
    // Use this for initialization
    void Start () {
      timeText.text = PlayerPrefs.GetString("time","00:00:000");
        deathText.text = "With " + PlayerPrefs.GetInt("deaths").ToString() + " Deaths";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
