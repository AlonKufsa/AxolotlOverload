using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorElement : GridElement
{
    private float glowIntensity = 5f;
    
    [SerializeField] private Renderer doorRenderer;
    private Material glowMaterial;
    
    private bool isDoorSteppedOn = false;
    
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

    public override void OnPlayerEnter(PlayerMovement playerScript, Vector2Int From)
    {
        
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2Int To)
    {
        
    }

    public void WhenDoorSteppedOn()
    {
        if (!isDoorSteppedOn) SetGlow(true);
        isDoorSteppedOn = true;
    }

    public void WhenDoorNotSteppedOn()
    {
        if (isDoorSteppedOn) SetGlow(false);
        isDoorSteppedOn = false;
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