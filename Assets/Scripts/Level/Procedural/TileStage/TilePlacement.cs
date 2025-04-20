using System.Linq;
using UnityEngine;

public record TilePlacement(LevelTile tile, Vector2Int position, int rotation)
{
    public Quaternion quaternion => Quaternion.Euler(0, rotation, 0);

    public LevelTile[] positiveX => rotation switch
    {
        0 => tile.positiveX.SelectMany(l => l.Value.identity).ToArray(),
        90 => tile.positiveZ.SelectMany(l => l.Value.identity).ToArray(),
        180 => tile.negativeX.SelectMany(l => l.Value.identity).ToArray(),
        270 => tile.negativeZ.SelectMany(l => l.Value.identity).ToArray(),
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] positiveZ => rotation switch
    {
        0 => tile.positiveZ.SelectMany(l => l.Value.identity).ToArray(),
        90 => tile.negativeX.SelectMany(l => l.Value.identity).ToArray(),
        180 => tile.negativeZ.SelectMany(l => l.Value.identity).ToArray(),
        270 => tile.positiveX.SelectMany(l => l.Value.identity).ToArray(),
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] negativeX => rotation switch
    {
        0 => tile.negativeX.SelectMany(l => l.Value.identity).ToArray(),
        90 => tile.negativeZ.SelectMany(l => l.Value.identity).ToArray(),
        180 => tile.positiveX.SelectMany(l => l.Value.identity).ToArray(),
        270 => tile.positiveZ.SelectMany(l => l.Value.identity).ToArray(),
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };

    public LevelTile[] negativeZ => rotation switch
    {
        0 => tile.negativeZ.SelectMany(l => l.Value.identity).ToArray(),
        90 => tile.positiveX.SelectMany(l => l.Value.identity).ToArray(),
        180 => tile.positiveZ.SelectMany(l => l.Value.identity).ToArray(),
        270 => tile.negativeX.SelectMany(l => l.Value.identity).ToArray(),
        _ => throw new System.IndexOutOfRangeException("Not an axis direction"),
    };
}
