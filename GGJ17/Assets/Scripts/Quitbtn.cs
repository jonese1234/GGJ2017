using UnityEngine;
using UnityEngine.UI;

public class Quitbtn : MonoBehaviour {

    public Button thisButton;

    // Use this for initialization
    void Start()
    {
        Button btn = thisButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }


    void TaskOnClick()
    {
        Application.Quit();
    }
}
