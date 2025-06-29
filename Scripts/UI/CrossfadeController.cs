using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CrossfadeController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject winScreen;
    
    private bool isLoading = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        GridManager.OnLevelFinished += OnLevelFinished;
        GridManager.OnLevelRestarted += OnLevelRestarted;
    }

    private void OnLevelRestarted()
    {
        if (!isLoading)
        {
            StartCoroutine(LoadLevel(false));
        }
        
    }

    private void OnLevelFinished()
    {
        isLoading = true;
        UIManager.CompletedLevels.Add(UIManager.CurrentLevelIndex);
        StartCoroutine(LoadLevel(true));
    }

    IEnumerator LoadLevel(bool incrementLevel)
    {
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");

        if (incrementLevel && levels.Length == UIManager.CurrentLevelIndex + 1)
        {
            winScreen.SetActive(true);
            
            yield break;
        }
        
        animator.SetTrigger("TrSwitchScene");
        
        GridManager.OnLevelFinished -= OnLevelFinished;
        GridManager.OnLevelRestarted -= OnLevelRestarted;
        
        yield return new WaitForSeconds(1f);
        
        
        if (levels.Length != UIManager.CurrentLevelIndex + 1)
        {
            if (incrementLevel)
            {
                UIManager.CurrentLevelIndex += 1;
                UIManager.CurrentLevelName = levels[UIManager.CurrentLevelIndex].name;
            }
            SceneManager.LoadScene(1, LoadSceneMode.Single);
            yield break;
        }

        // TODO: Win screen
    }

    private void OnDestroy()
    {
        GridManager.OnLevelFinished -= OnLevelFinished;
        GridManager.OnLevelRestarted -= OnLevelRestarted;
    }
}
