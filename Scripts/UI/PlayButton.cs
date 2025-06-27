using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnClick()
    {
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");

        UIManager.CurrentLevelIndex = 0;
        UIManager.CurrentLevelName = levels[0].name;
        
        SceneManager.LoadScene(1);
    }
}
