using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorElement : GridElement
{
    private float glowIntensity = 5f;
    
    public static event Action OnPlayerReachedDoor;
    
    [SerializeField] private Renderer doorRenderer;
    private Material glowMaterial;
    
    private bool isDoorOpen = false;

    public static void ClearAllSubscribers()
    {
        OnPlayerReachedDoor = null;
    }
    
    public override bool GetIsWalkabilityDependant()
    {
        return true;
    }

    public override bool GetIsNeverWalkableWhenSmall()
    {
        return false;
    }

    public override bool GetIsNeverWalkableWhenNormal()
    {
        return false;
    }

    public override void OnPlayerEnter(PlayerMovement playerScript, Vector2 From)
    {
        OnPlayerReachedDoor?.Invoke();
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2 To)
    {
        
    }

    private void EnsureInitialized()
    {
        if (glowMaterial == null)
        {
            var materials = doorRenderer.materials;
            glowMaterial = new Material(materials[1]);
            materials[1] = glowMaterial;
            doorRenderer.materials = materials;
        }
    }

    public void OpenDoor()
    {
        isDoorOpen = true;
        SetGlow(true);
    }

    public void CloseDoor()
    {
        if (isDoorOpen)
        {
            isDoorOpen = false;
            SetGlow(false);
        }
    }

    private void SetGlow(bool shouldGlow)
    {
        EnsureInitialized();
        
        if (shouldGlow)
        {
            glowMaterial.EnableKeyword("_EMISSION");
            glowMaterial.SetColor("_EmissionColor", Color.cyan * glowIntensity);
        }
        else
        {
            glowMaterial.SetColor("_EmissionColor", Color.black);
            glowMaterial.DisableKeyword("_EMISSION");
        }
    }

    private void Start()
    {
        var materials = doorRenderer.materials;
        glowMaterial = new Material(materials[1]);
        materials[1] = glowMaterial;
        doorRenderer.materials = materials;
        
        SetGlow(false);
    }
}