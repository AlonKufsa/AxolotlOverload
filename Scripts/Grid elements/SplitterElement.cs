using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SplitterMode
{
    Vertical,
    Horizontal,
    Both,
}

public class SplitterElement : GridElement
{
    public SplitterMode splitterMode = SplitterMode.Both;
    
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
        if (!playerScript.isSmall)
        {
            foreach (var position in GetPositionsToSplitTo(From))
            {
                gridManager.AddPlayer(position, true);
            }
        }
        
        gridManager.KillPlayer(playerScript);
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2Int To)
    {
        
    }
    
    public List<Vector2Int> GetPositionsToSplitTo(Vector2Int originalPosition)
    {
        var list = new List<Vector2Int>();

        if (splitterMode == SplitterMode.Vertical || (splitterMode == SplitterMode.Both && Position.x == originalPosition.x))
        {
            list.Add(Position - new Vector2Int(1, 0));
            list.Add(Position + new Vector2Int(1, 0));
        } else if (splitterMode == SplitterMode.Horizontal || (splitterMode == SplitterMode.Both && Position.y == originalPosition.y))
        {
            list.Add(Position - new Vector2Int(0, 1));
            list.Add(Position + new Vector2Int(0, 1));
        }

        return list;
    }
}
