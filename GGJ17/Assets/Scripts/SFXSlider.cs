using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour {

    public Slider thisSlider;
    // Use this for initialization
    void Start()
    {
        Slider slide = thisSlider.GetComponent<Slider>();
        slide.onValueChanged.AddListener(delegate { TaskOnChange(); });
        thisSlider = slide;
        thisSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1.0f)*100;
    }

    void TaskOnChange()
    {
        PlayerPrefs.SetFloat("sfxVolume", thisSlider.value / 100f);
        PlayerPrefs.Save();
    }
}
