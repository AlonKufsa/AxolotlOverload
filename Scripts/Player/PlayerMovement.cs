using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum Direction {Up, Down, Left, Right}

public struct MoveIntent
{
    public Vector2 From;
    public Vector2 To;
    public PlayerMovement Controller;
}
public class PlayerMovement : MonoBehaviour
{
    public static event Action<Vector2, Vector2, PlayerMovement> OnPlayerMoved;
    
    [SerializeField] private Animator animator;
    
    private int currentX = 0;
    private int currentZ = 0;
    private Direction currentDirection = Direction.Right;
    [HideInInspector] public bool isAllowedToMove = false;
    private bool isMoving = false;
    
    private int wantedTargetX = 0;
    private int wantedTargetZ = 0;

    private int currentTargetX = 0;
    private int currentTargetZ = 0;

    public bool isSmall = false;
    
    public void HandleInput()
    {
        var keyboard = Keyboard.current;

        var horizontal = 0;
        var vertical = 0;
        if (keyboard.dKey.wasPressedThisFrame && !keyboard.aKey.wasPressedThisFrame) horizontal = 1;
        else if (keyboard.aKey.wasPressedThisFrame && !keyboard.dKey.wasPressedThisFrame) horizontal = -1;
        else if (keyboard.wKey.wasPressedThisFrame && !keyboard.sKey.wasPressedThisFrame) vertical = 1;
        else if (keyboard.sKey.wasPressedThisFrame && !keyboard.wKey.wasPressedThisFrame) vertical = -1;

        if (vertical == 0 && horizontal != 0)
        {
            if (!isMoving)
            {
                currentDirection = (horizontal > 0)? Direction.Right : Direction.Left;
                UpdateWantedTargetTile(currentDirection);
            }
        } else if (horizontal == 0 && vertical != 0)
        {
            if (!isMoving)
            {
                currentDirection = (vertical > 0)? Direction.Up : Direction.Down;
                UpdateWantedTargetTile(currentDirection);
            }
        }
    }

    private void calculatePlayerPositionOnGrid()
    {
        currentX = (int)Math.Round(transform.position.x / GameConstants.TileSize - 0.5);
        currentZ = (int)Math.Round(transform.position.z / GameConstants.TileSize - 0.5);
    }

    private void Start()
    {
        calculatePlayerPositionOnGrid();
        wantedTargetX = currentX;
        wantedTargetZ = currentZ;
        currentTargetX = currentX;
        currentTargetZ = currentZ;
    }

    private void FaceDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                transform.eulerAngles = new Vector3(0, -90f, 0);
                break;
            case Direction.Down:
                transform.eulerAngles = new Vector3(0, 90f, 0);
                break;
            case Direction.Left:
                transform.eulerAngles = new Vector3(0, 180f, 0);
                break;
            case Direction.Right:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    private void SetAnimationToWalk()
    {
        animator.SetTrigger("TrStartWalking");
    }

    private void SetAnimationToIdle()
    {
        animator.SetTrigger("TrStopWalking");
    }

    private float GetDistanceToCurrentTargetTile()
    {
        return Vector3.Distance(transform.position, GridSystem.GetPositionAtCoordinates(new Vector2(currentTargetX, currentTargetZ)) + Vector3.up * transform.position.y);
    }
    
    private float GetDistanceToWantedTargetTile()
    {
        return Vector3.Distance(transform.position, GridSystem.GetPositionAtCoordinates(new Vector2(wantedTargetX, wantedTargetZ)) + Vector3.up * transform.position.y);
    }

    private void UpdateWantedTargetTile(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                wantedTargetX = currentX;
                wantedTargetZ = currentZ + 1;
                break;
            case Direction.Down:
                wantedTargetX = currentX;
                wantedTargetZ = currentZ - 1;
                break;
            case Direction.Left:
                wantedTargetX = currentX - 1;
                wantedTargetZ = currentZ;
                break;
            case Direction.Right:
                wantedTargetX = currentX + 1;
                wantedTargetZ = currentZ;
                break;
        }
    }
    
    public MoveIntent GetMoveIntent()
    {
        MoveIntent moveIntent = new MoveIntent();
        moveIntent.From = new Vector2(currentX, currentZ);
        
        
        moveIntent.To = new Vector2(wantedTargetX, wantedTargetZ);

        moveIntent.Controller = this;
        
        return moveIntent;
    }
    
    public bool IsFinishedMoving() { return !isMoving; }

    private void MoveToCurrentTargetTile()
    {
        transform.position = Vector3.MoveTowards(transform.position, GridSystem.GetPositionAtCoordinates(new Vector2(currentTargetX, currentTargetZ)) + Vector3.up * transform.position.y, GameConstants.PlayerMovementSpeed * Time.deltaTime);
        
        if (isMoving &&
            GetDistanceToCurrentTargetTile() < GameConstants.SnapDistance)
        {
            int previousX = currentX;
            int previousZ = currentZ;
            
            currentX = currentTargetX;
            currentZ = currentTargetZ;
            
            OnPlayerMoved?.Invoke(new Vector2(currentTargetX, currentTargetZ), new Vector2(previousX, previousZ), this);
            
            isMoving = false;
            isAllowedToMove = false;
            
        }
    }

    private void Update()
    {
        FaceDirection(currentDirection);
        if (isAllowedToMove)
        {
            currentTargetX = wantedTargetX;
            currentTargetZ = wantedTargetZ;
            
            isMoving = true;
        }
        else
        {
            currentTargetX = currentX;
            currentTargetZ = currentZ;
        }
        MoveToCurrentTargetTile();

        if (isMoving)
        {
            SetAnimationToWalk();
        }
        else
        {
            SetAnimationToIdle();
        }
        
        isAllowedToMove = false;
    }
}
