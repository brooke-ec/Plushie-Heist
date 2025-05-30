using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public record TileNeighbors(
    Vector2Int position,
    TileNeighbour positiveX,
    TileNeighbour positiveZ,
    TileNeighbour negativeX,
    TileNeighbour negativeZ
)
{
    private LevelTile[] Union()
    {
        List<LevelTile> union = new List<LevelTile>();
        union.AddRange(positiveX.negativeX);
        union.AddRange(positiveZ.negativeZ);
        union.AddRange(negativeX.positiveX);
        union.AddRange(negativeZ.positiveZ);
        return union.Where(t => t != null && !t.exclude).ToArray();
    }

    public bool IsValid(TilePlacement tile)
    {
        return positiveX.IsApplicable(tile.positiveX)
            && positiveZ.IsApplicable(tile.positiveZ)
            && negativeX.IsApplicable(tile.negativeX)
            && negativeZ.IsApplicable(tile.negativeZ);
    }

    public TilePlacement[] Possible()
    {
        LevelTile[] union = Union();
        List<TilePlacement> possible = new List<TilePlacement>();

        for (int rotation = 0; rotation < 360; rotation += 90)
            foreach (LevelTile tile in union)
            {
                TilePlacement placement = new TilePlacement(tile, position, rotation);
                if (IsValid(placement)) possible.Add(placement);
            }

        return possible.ToArray();
    }

    public override string ToString()
    {
        return string.Format("Clockwise Neighbor Requirements: \n{0},\n{1},\n{2},\n{3}",
            positiveZ.isWall ? "WALL" : positiveZ.isAnyTile ? "ANY TILE" : LevelTile.TilesToString(positiveZ.negativeZ),
            positiveX.isWall ? "WALL" : positiveX.isAnyTile ? "ANY TILE" : LevelTile.TilesToString(positiveX.negativeX),
            negativeZ.isWall ? "WALL" : negativeZ.isAnyTile ? "ANY TILE" : LevelTile.TilesToString(negativeZ.positiveZ),
            negativeX.isWall ? "WALL" : negativeX.isAnyTile ? "ANY TILE" : LevelTile.TilesToString(negativeX.positiveX)
        );
    }
}