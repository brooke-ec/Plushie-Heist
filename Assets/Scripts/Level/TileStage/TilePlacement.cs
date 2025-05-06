using UnityEngine;

public record TilePlacement(LevelTile tile, Vector2Int position, int rotation)
{
    public Quaternion quaternion => Quaternion.Euler(0, rotation, 0);

    public LevelTile[] positiveX => rotation switch
    {
        0 => tile.positiveX,
        90 => tile.positiveZ,
        180 => tile.negativeX,
        270 => tile.negativeZ,
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] positiveZ => rotation switch
    {
        0 => tile.positiveZ,
        90 => tile.negativeX,
        180 => tile.negativeZ,
        270 => tile.positiveX,
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] negativeX => rotation switch
    {
        0 => tile.negativeX,
        90 => tile.negativeZ,
        180 => tile.positiveX,
        270 => tile.positiveZ,
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] negativeZ => rotation switch
    {
        0 => tile.negativeZ,
        90 => tile.positiveX,
        180 => tile.positiveZ,
        270 => tile.negativeX,
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };
}
