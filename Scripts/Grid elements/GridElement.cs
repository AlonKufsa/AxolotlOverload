using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridElement : MonoBehaviour
{
    [HideInInspector] public GridManager gridManager;
    [HideInInspector] public Vector2 Position;

    public abstract bool GetIsWalkabilityDependant();
    
    public abstract bool GetIsNeverWalkableWhenSmall();
    public abstract bool GetIsNeverWalkableWhenNormal();

    public abstract void OnPlayerEnter(PlayerMovement playerScript, Vector2 From);
    public abstract void OnPlayerExit(PlayerMovement playerScript, Vector2 To);
}
