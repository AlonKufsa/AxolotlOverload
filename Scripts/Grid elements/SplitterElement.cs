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
    [SerializeField] private SplitterMode splitterMode = SplitterMode.Both;
    
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
        if (!playerScript.isSmall)
        {
            foreach (var position in GetPositionsToSplitTo(From))
            {
                gridManager.AddPlayer(position, true);
            }
        }
        
        gridManager.KillPlayer(playerScript);
    }

    public override void OnPlayerExit(PlayerMovement playerScript, Vector2 To)
    {
        
    }
    
    public List<Vector2> GetPositionsToSplitTo(Vector2 originalPosition)
    {
        var list = new List<Vector2>();

        if (splitterMode == SplitterMode.Vertical || (splitterMode == SplitterMode.Both && Mathf.Approximately(originalPosition.x, Position.x)))
        {
            list.Add(Position - new Vector2(1, 0));
            list.Add(Position + new Vector2(1, 0));
        } else if (splitterMode == SplitterMode.Horizontal || (splitterMode == SplitterMode.Both && Mathf.Approximately(originalPosition.y, Position.y)))
        {
            list.Add(Position - new Vector2(0, 1));
            list.Add(Position + new Vector2(0, 1));
        }

        return list;
    }
}
