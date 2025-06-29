using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridElement : MonoBehaviour
{
    [HideInInspector] public GridManager gridManager;
    [HideInInspector] public Vector2Int Position;

    public abstract bool GetIsWalkabilityDependant();
    
    public abstract bool GetIsNeverWalkableWhenSmall();
    public abstract bool GetIsNeverWalkableWhenNormal();

    public abstract void OnPlayerEnter(PlayerMovement playerScript, Vector2Int From);
    public abstract void OnPlayerExit(PlayerMovement playerScript, Vector2Int To);
}
