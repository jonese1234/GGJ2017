using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip[] soundtrack;

	// Use this for initialization
	void Start () {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        if (!GetComponent<AudioSource>().playOnAwake)
        {            
            GetComponent<AudioSource>().clip = soundtrack[Random.Range(0, soundtrack.Length - 1)];
            GetComponent<AudioSource>().Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = soundtrack[Random.Range(0, soundtrack.Length - 1)];
            GetComponent<AudioSource>().Play();
        }
	}
}
