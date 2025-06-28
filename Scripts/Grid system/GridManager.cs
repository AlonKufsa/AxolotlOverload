using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GridManager detected, destroying!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    [SerializeField] private GameObject darkTilePrefab;
    [SerializeField] private GameObject brightTilePrefab;
    [SerializeField] private PlayerMovement playerPrefab;
    [SerializeField] private TestElement testGridElementPrefab;
    [SerializeField] private WallElement wallGridElementPrefab;
    [SerializeField] private SpikeElement spikeGridElementPrefab;
    [SerializeField] private GrowElement growGridElementPrefab;
    [SerializeField] private SplitterElement BothSplitterGridElementPrefab;
    [SerializeField] private SplitterElement HorizontalSplitterGridElementPrefab;
    [SerializeField] private SplitterElement VerticalSplitterGridElementPrefab;
    [SerializeField] private ButtonElement buttonGridElementPrefab;
    [SerializeField] private DoorElement doorGridElementPrefab;

    
    [SerializeField] private Transform cameraTransform;

    private GridSystem grid;

    private Dictionary<Vector2, GridElement> gridElements = new Dictionary<Vector2, GridElement>();
    private List<PlayerMovement> playerControllers = new List<PlayerMovement>();
    
    private List<MoveIntent> moveIntents = new List<MoveIntent>();

    private Dictionary<char, GridElement> charGridElements;
    
    private bool isLevelFinished = false;
    void Start()
    {
        PlayerMovement.OnPlayerMoved += OnPlayerMoved;
        
        charGridElements = new Dictionary<char, GridElement>()
        {
            {'D', doorGridElementPrefab},
            {'B', buttonGridElementPrefab},
            {'X', spikeGridElementPrefab},
            {'#', wallGridElementPrefab},
            {'G', growGridElementPrefab},
            {'+', BothSplitterGridElementPrefab},
            {'-', HorizontalSplitterGridElementPrefab},
            {'|', VerticalSplitterGridElementPrefab},
        };
        LoadLevel(UIManager.CurrentLevelName);
        
        PositionCamera();
        
        CrossfadeController.OnCrossfadeStarted += OnCrossfadeStarted;
    }

    private void OnDestroy()
    {
        CrossfadeController.OnCrossfadeStarted -= OnCrossfadeStarted;
        PlayerMovement.OnPlayerMoved -= OnPlayerMoved;
    }

    // GRID GENERATION

    private void LoadLevel(String levelName)
    {
        playerControllers.Clear();
        gridElements.Clear();
        isLevelFinished = false;
        
        TextAsset textAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
        string[] lines = textAsset.text.Split('\n').Reverse().ToArray();
        
        GenerateGrid(lines[0].Length, lines.Length);

        for (int z = 0; z < lines.Length; z++)
        {
            for (int x = 0; x < lines[z].Length; x++)
            {
                char c = lines[z][x];
                if (charGridElements.ContainsKey(c))
                {
                    var gridElement = charGridElements[c];
                    Vector2Int gridPosition = new Vector2Int(x, z);
                    
                    AddGridElement(gridElement, gridPosition);
                } else if (c == 'P')
                {
                    Vector2Int gridPosition = new Vector2Int(x, z);
                    AddPlayer(gridPosition, false);
                } else if (c == 'p')
                {
                    Vector2Int gridPosition = new Vector2Int(x, z);
                    AddPlayer(gridPosition, true);
                }
            }
        }
    }
    
    private void GenerateGrid(int xLength, int zLength)
    {
        grid = new GridSystem(xLength, zLength);
        for (int x = 0; x < xLength; x++)
        {
            for (int z = 0; z < zLength; z++)
            {
                if (x % 2 == z % 2)
                {
                    GameObject tile = Instantiate(darkTilePrefab, GridSystem.GetPositionAtCoordinates(new Vector2(x, z)), Quaternion.identity);
                    tile.transform.localScale = new Vector3(GameConstants.TileSize, 1f, GameConstants.TileSize);
                }
                else
                {
                    GameObject tile = Instantiate(brightTilePrefab, GridSystem.GetPositionAtCoordinates(new Vector2(x, z)), Quaternion.identity);
                    tile.transform.localScale = new Vector3(GameConstants.TileSize, 1f, GameConstants.TileSize);
                }
            }
        }
    }

    private void PositionCamera()
    {
        cameraTransform.position = new Vector3((grid.xLength * GameConstants.TileSize)/2f, cameraTransform.position.y, (grid.zLength * GameConstants.TileSize)/2f);
    }

    private void PlaceGameObjectOnGrid(GameObject obj, Vector2 gridPosition)
    {
        obj.transform.localScale *= GameConstants.TileSize;
        obj.transform.position = GridSystem.GetPositionAtCoordinates(gridPosition) + Vector3.up * ((2 * obj.transform.position.y * GameConstants.TileSize - GameConstants.TileSize + 1) / 2f);
    }
    
    private bool GetGridElementAt(Vector2 gridPosition, out GridElement gridElement)
    {
        return gridElements.TryGetValue(gridPosition, out gridElement);
    }
    
    // Prefab must have a script deriving from GridElement!!!
    private void AddGridElement(GridElement gridElementPrefab, Vector2 gridPosition)
    {
        gridElements.Add(gridPosition, Instantiate(gridElementPrefab));
        PlaceGameObjectOnGrid(gridElements.Last().Value.gameObject, gridPosition);
        gridElements.Last().Value.gridManager = this;
        gridElements.Last().Value.Position = gridPosition;
    }
    
    public void AddPlayer(Vector2 gridPosition, bool isSmall)
    {
        playerControllers.Add(Instantiate(playerPrefab));
        PlaceGameObjectOnGrid(playerControllers.Last().gameObject, gridPosition);
        playerControllers.Last().isSmall = false;
        if (isSmall)
        {
            ResizePlayer(playerControllers.Last());
        }
    }
    
    // GENERAL GAME FUNCTIONALITY

    public void ResizePlayer(PlayerMovement playerScript)
    {
        if (playerScript.isSmall)
        {
            playerScript.isSmall = false;
            
            playerScript.transform.localScale = playerPrefab.transform.localScale * GameConstants.TileSize;
            playerScript.transform.position = new Vector3(
                playerScript.transform.position.x,
                (2 * playerPrefab.transform.position.y * GameConstants.TileSize - GameConstants.TileSize + 1) / 2f,
                playerScript.transform.position.z
                );
        }
        else
        {
            playerScript.isSmall = true;
            
            playerScript.transform.localScale *= GameConstants.BigToSmallMultiplier;
            playerScript.transform.position += new Vector3(0f, -playerScript.transform.position.y + (2*playerScript.transform.position.y*GameConstants.BigToSmallMultiplier - GameConstants.BigToSmallMultiplier + 1f)/2f, 0f);
        }
    }

    public void KillPlayer(PlayerMovement playerScript)
    {
        playerControllers.Remove(playerScript);
        Destroy(playerScript.gameObject);
    }

    // MOVEMENT PROCESSING
    
    private void AddMoveIntent(MoveIntent moveIntent)
    {
        moveIntents.Add(moveIntent);
    }

    private void ClearMoveIntents()
    {
        moveIntents.Clear();
    }

    private bool IsMoveValidInGrid(MoveIntent moveIntent)
    {
        if (moveIntent.To.x >= grid.xLength || moveIntent.To.y >= grid.zLength) return false;
        if (moveIntent.To.x < 0 || moveIntent.To.y < 0) return false;
        if (moveIntent.To == moveIntent.From) return false;

        GridElement gridElement;
        if (!GetGridElementAt(moveIntent.To, out gridElement)) return true;

        if (gridElement.GetIsWalkabilityDependant()) return true;
        
        if (gridElement.GetIsNeverWalkableWhenNormal() && !moveIntent.Controller.isSmall) return false;
        if (gridElement.GetIsNeverWalkableWhenSmall() && moveIntent.Controller.isSmall) return false;
        
        return true;
    }

    // Leaves only valid move intents in the move intents list
    private void ProcessMoveIntents()
    {
        if (moveIntents.Count == 0) return;
        
        var validMoveIntents = new List<MoveIntent>();
        var invalidMoveIntents = new List<MoveIntent>();

        var occupiedPositions = new List<Vector2>();
        
        foreach (var intent in moveIntents)
        {
            if (IsMoveValidInGrid(intent))
            {
                validMoveIntents.Add(intent);
            }
            else
            {
                invalidMoveIntents.Add(intent);
            }
        }
        
        occupiedPositions.AddRange(validMoveIntents.Select(intent => intent.To));
        occupiedPositions.AddRange(invalidMoveIntents.Select(intent => intent.From));
        
        
        foreach (var splitterElement in gridElements.Values.OfType<SplitterElement>())
        {
            var moveIntentsGoingIn = validMoveIntents.Where(moveIntent => moveIntent.To == splitterElement.Position).ToList();
            if (moveIntentsGoingIn.Count == 0) continue;
            
            
            var splitPositions = splitterElement.GetPositionsToSplitTo(moveIntentsGoingIn[0].From);
            if (occupiedPositions.Any(position => splitPositions.Contains(position)))
            {
                validMoveIntents.RemoveAll(intent => intent.To == splitterElement.Position && intent.Controller.isSmall == false);
                if (!moveIntentsGoingIn[0].Controller.isSmall) invalidMoveIntents.Add(moveIntentsGoingIn[0]);
            }
            
        }
        occupiedPositions.Clear();
        occupiedPositions.AddRange(validMoveIntents.Select(intent => intent.To));
        occupiedPositions.AddRange(invalidMoveIntents.Select(intent => intent.From));
        
        // Check if 2 players will collide
        var safety = 0;
        while (occupiedPositions.Distinct().Count() != occupiedPositions.Count && safety < 100)
        {
            safety++;
            List<Vector2> duplicatePositions = new List<Vector2>();
            HashSet<Vector2> occupiedPositionsSet = new HashSet<Vector2>();
            foreach (var position in occupiedPositions)
            {
                if (!occupiedPositionsSet.Add(position)) duplicatePositions.Add(position);
            }
            //occupiedPositions.AddRange(validMoveIntents.Where(intent => duplicatePositions.Contains(intent.To)).ToList().Select(intent => intent.From).ToList());
            invalidMoveIntents.AddRange(validMoveIntents.Where(intent => duplicatePositions.Contains(intent.To)));
            validMoveIntents.RemoveAll(intent => duplicatePositions.Contains(intent.To));
            occupiedPositions.Clear();
            occupiedPositions.AddRange(validMoveIntents.Select(intent => intent.To));
            occupiedPositions.AddRange(invalidMoveIntents.Select(intent => intent.From));
        }


        var allButtonsPressed = true;
        foreach (var button in gridElements.Values.OfType<ButtonElement>())
        {
            if (!occupiedPositions.Contains(button.Position))
            {
                button.ReleaseButton();
                allButtonsPressed = false;
            }
            else
            {
                button.PressButton();
            }
        }

        if (allButtonsPressed)
        {
            foreach (var door in gridElements.Values.OfType<DoorElement>())
            {
                door.OpenDoor();
            }
        }
        else
        {
            foreach (var door in gridElements.Values.OfType<DoorElement>())
            {
                door.CloseDoor();
                if (validMoveIntents.Any(intent => intent.To == door.Position))
                {
                    invalidMoveIntents.AddRange(validMoveIntents.Where(intent => intent.To == door.Position));
                    validMoveIntents.RemoveAll(intent => intent.To == door.Position);
                    
                    occupiedPositions.Clear();
                    occupiedPositions.AddRange(validMoveIntents.Select(intent => intent.To));
                    occupiedPositions.AddRange(invalidMoveIntents.Select(intent => intent.From));
                    
                    // Check if 2 players will collide
                    safety = 0;
                    while (occupiedPositions.Distinct().Count() != occupiedPositions.Count && safety < 100)
                    {
                        safety++;
                        List<Vector2> duplicatePositions = new List<Vector2>();
                        HashSet<Vector2> occupiedPositionsSet = new HashSet<Vector2>();
                        foreach (var position in occupiedPositions)
                        {
                            if (!occupiedPositionsSet.Add(position)) duplicatePositions.Add(position);
                        }
                        //occupiedPositions.AddRange(validMoveIntents.Where(intent => duplicatePositions.Contains(intent.To)).ToList().Select(intent => intent.From).ToList());
                        invalidMoveIntents.AddRange(validMoveIntents.Where(intent => duplicatePositions.Contains(intent.To)));
                        validMoveIntents.RemoveAll(intent => duplicatePositions.Contains(intent.To));
                        occupiedPositions.Clear();
                        occupiedPositions.AddRange(validMoveIntents.Select(intent => intent.To));
                        occupiedPositions.AddRange(invalidMoveIntents.Select(intent => intent.From));
                    }
                }
            }
        }
        
        moveIntents = validMoveIntents;
    }

    private void OnPlayerMoved(Vector2 newCoordinates, Vector2 previousCoordinates, PlayerMovement player)
    {
        GridElement gridElementAtPreviousLocation;
        GridElement gridElementAtNewLocation;

        bool hasNew = GetGridElementAt(newCoordinates, out gridElementAtNewLocation);
        bool hasPrevious = GetGridElementAt(previousCoordinates, out gridElementAtPreviousLocation);
        
        if (!hasNew && !hasPrevious) return;
        
        if (hasNew) gridElementAtNewLocation?.OnPlayerEnter(player, previousCoordinates);
        if (hasPrevious) gridElementAtPreviousLocation?.OnPlayerExit(player, newCoordinates);
    }

    private void OnCrossfadeStarted()
    {
        isLevelFinished = true;
    }

    private void Update()
    {
        if (!isLevelFinished)
        {
            var allPlayersReady = true;
            foreach (var player in playerControllers)
            {
                if (!player.IsFinishedMoving()) allPlayersReady = false;
            }

            foreach (var player in playerControllers)
            {
                if (allPlayersReady) player.HandleInput();
                AddMoveIntent(player.GetMoveIntent());
            }

            ProcessMoveIntents();

            foreach (var moveIntent in moveIntents)
            {
                moveIntent.Controller.isAllowedToMove = true;
            }

            ClearMoveIntents();
        }
    }
}
