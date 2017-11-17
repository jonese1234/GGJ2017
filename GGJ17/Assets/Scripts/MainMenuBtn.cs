using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuBtn : MonoBehaviour {

    public Button thisButton;
    // Use this for initialization
    void Start()
    {
        Button btn = thisButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }


    void TaskOnClick()
    {
        SceneManager.LoadScene(0);
    }
}
