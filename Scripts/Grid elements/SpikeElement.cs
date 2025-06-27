using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeElement : GridElement
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
        gridManager.KillPlayer(playerScript);
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2 To)
    {
        
    }
}
