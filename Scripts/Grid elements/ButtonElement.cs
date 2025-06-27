using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElement : GridElement
{
    [SerializeField] private Animator animator;
    public override bool GetIsWalkabilityDependant()
    {
        return false;
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
        
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2 To)
    {
        
    }

    public void PressButton()
    {
        animator.SetTrigger("TrPressButton");

    }

    public void ReleaseButton()
    {
        animator.SetTrigger("TrDepressButton");
    }
}
