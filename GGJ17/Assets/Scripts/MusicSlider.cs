using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MusicSlider : MonoBehaviour {

    public Slider thisSlider; 
	// Use this for initialization
	void Start () {
        Slider slide = thisSlider.GetComponent<Slider>();
        slide.onValueChanged.AddListener(delegate { TaskOnChange(); });
        thisSlider = slide;
        thisSlider.value = PlayerPrefs.GetFloat("musicVolume", 1.0f)*100;
    }
	
	void TaskOnChange () {
        PlayerPrefs.SetFloat("musicVolume", thisSlider.value / 100f);
        PlayerPrefs.Save(); 
	}
}
