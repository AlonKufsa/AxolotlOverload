using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowElement : GridElement
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

    public override void OnPlayerEnter(PlayerMovement playerScript, Vector2Int From)
    {
        if (playerScript.isSmall)
        {
            gridManager.ResizePlayer(playerScript);
        }
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2Int To)
    {
        
    }
}
