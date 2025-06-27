using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestElement : GridElement
{
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
        Debug.Log("I was entered");
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2 To)
    {
        Debug.Log("I was exited");
    }
}
