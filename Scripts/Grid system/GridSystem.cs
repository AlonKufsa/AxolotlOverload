using UnityEngine;

public class GridSystem
{
    public int xLength {get; private set;}
    public int zLength { get; private set; }

    public GridSystem(int xLength, int zLength)
    {
        this.xLength = xLength;
        this.zLength = zLength;
    }

    public static Vector3 GetPositionAtCoordinates(Vector2 coordinates)
    {
        return new Vector3(coordinates.x * GameConstants.TileSize + (0.5f * GameConstants.TileSize), 0f, coordinates.y * GameConstants.TileSize  + (0.5f * GameConstants.TileSize));
    }
}
