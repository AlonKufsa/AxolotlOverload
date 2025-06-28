using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;

    private void Start()
    {
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");

        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            
            GameObject levelButton = Instantiate(buttonPrefab, transform);
            
            var text = levelButton.GetComponentInChildren<TextMeshProUGUI>();
            text.text = (i + 1).ToString();

            var checkmark = levelButton.transform.GetChild(1);
            if (UIManager.CompletedLevels.Contains(i)) checkmark.gameObject.SetActive(true);

            levelButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.CurrentLevelName = levels[index].name;
                UIManager.CurrentLevelIndex = index;

                SceneManager.LoadScene(1);
            });
        }
    }
}
