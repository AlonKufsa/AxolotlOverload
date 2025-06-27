using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CrossfadeController : MonoBehaviour
{
    public static event Action OnCrossfadeStarted;
    
    private Animator animator;
    
    private bool isLoading = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        DoorElement.OnPlayerReachedDoor += OnPlayerReachedDoor;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.rKey.wasPressedThisFrame)
        {
            if (!isLoading)
            {
                StartCoroutine(LoadLevel(false));
            }
        }
    }

    private void OnPlayerReachedDoor()
    {
        isLoading = true;
        StartCoroutine(LoadLevel(true));
    }

    IEnumerator LoadLevel(bool incrementLevel)
    {
        OnCrossfadeStarted?.Invoke();
        animator.SetTrigger("TrSwitchScene");
        
        yield return new WaitForSeconds(1f);
        
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels");
        
        if (incrementLevel && levels.Length != UIManager.CurrentLevelIndex + 1)
        {
            UIManager.CurrentLevelIndex += 1;
            UIManager.CurrentLevelName = levels[UIManager.CurrentLevelIndex].name;
        }

        SceneManager.LoadScene(2);
    }

    private void OnDestroy()
    {
        DoorElement.OnPlayerReachedDoor -= OnPlayerReachedDoor;
    }
}
