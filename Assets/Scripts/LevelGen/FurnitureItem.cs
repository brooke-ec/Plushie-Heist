using System.Collections.Generic;
using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public Vector2Int size;

    public Region region { get; private set; }
    public FurnitureGrid grid { get; private set; }

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
        transform.Rotate(0, 90, 0);
    }

    public void Place(FurnitureGrid grid, Vector2 coordinates)
    {
        this.grid = grid;
        region = Region.FromSize(
            Mathf.FloorToInt(coordinates.x),
            Mathf.FloorToInt(coordinates.y),
            size.x, size.y
        );

        if (region.Within(grid.size)) transform.position = grid.ToWorldspace(region.center);
    }

    public bool IsValid()
    {
        return grid != null && region != null && region.Within(grid.size) && !grid.Intersects(region);
    }
}
