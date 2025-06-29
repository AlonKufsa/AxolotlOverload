using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallElement : GridElement
{
    public override bool GetIsWalkabilityDependant()
    {
        return false;
    }

    public override bool GetIsNeverWalkableWhenSmall()
    {
        return true;
    }

    public override bool GetIsNeverWalkableWhenNormal()
    {
        return true;
    }

    public override void OnPlayerEnter(PlayerMovement playerScript, Vector2Int From)
    {
        
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2Int To)
    {
        
    }
}
