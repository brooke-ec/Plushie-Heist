using System.Linq;

public record TileNeighbour(TilePlacement placement, bool isWall)
{
    public LevelTile[] positiveX => placement == null ? new LevelTile[0] : placement.positiveX;
    public LevelTile[] positiveZ => placement == null ? new LevelTile[0] : placement.positiveZ;
    public LevelTile[] negativeX => placement == null ? new LevelTile[0] : placement.negativeX;
    public LevelTile[] negativeZ => placement == null ? new LevelTile[0] : placement.negativeZ;
    public bool isAnyTile => placement == null;

    public bool IsApplicable(LevelTile[] tiles)
    {
        if (tiles.Length == 0) return isWall;
        if (isWall) return tiles.Length == 0;
        return isAnyTile || tiles.Contains(placement.tile);
    }
}