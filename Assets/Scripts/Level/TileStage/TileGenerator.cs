using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileGenerator
{
    [field: SerializeField] public float tileSize { get; private set; } = 10;
    [SerializeField] private LevelTile startTile;
    [SerializeField] private LevelTile endTile;
    
    private Vector3 physicalSize => new Vector3(size.x - 1, 0, size.y - 1) * tileSize;

    private Dictionary<Vector2Int, TilePlacement> grid = new Dictionary<Vector2Int, TilePlacement>();
    private Queue<Vector2Int> toPlace = new Queue<Vector2Int>();
    private Transform parent;
    private Vector2Int size;

    public void Generate(Region[] regions, Transform parent, Vector2Int size)
    {
        this.parent = parent;
        this.size = size;

        regions.ForEach(r => r.ForEach(p => grid[p] = null));
        PlaceTiles();
    }

    private void PlaceTiles()
    {
        PlaceTile(startTile, Vector2Int.one,(int)startTile.transform.rotation.eulerAngles.y);
        PlaceTile(endTile, size - Vector2Int.one, (int)endTile.transform.rotation.eulerAngles.y);

        while (toPlace.Count > 0)
        {
            Vector2Int position = toPlace.Dequeue();
            if (grid[position] != null) continue;

            Vector2Int v;
            TileNeighbors neighbors = new TileNeighbors(
                position,
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(+1, 0)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(0, +1)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(-1, 0)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(0, -1)), !grid.ContainsKey(v))
            );

            TilePlacement[] possible = neighbors.Possible();
            if (possible.Length > 0) PlaceTile(possible[Random.Range(0, possible.Length)]);
            else throw new System.IndexOutOfRangeException(
                $"Found no options for placing tile. {neighbors}\n\n"
            );
        }
    }

    private void PlaceTile(LevelTile tile, Vector2Int position, int rotation)
    {
        PlaceTile(new TilePlacement(tile, position, rotation));
    }
    


    private void PlaceTile(TilePlacement placement)
    {
        grid[placement.position] = placement;
        Vector3 position = new Vector3(placement.position.x - .5f, 0, placement.position.y - .5f) * tileSize - physicalSize / 2;
        GameObject.Instantiate(placement.tile, position, placement.quaternion, parent);

        Vector2Int v;
        if (grid.ContainsKey(v = placement.position + new Vector2Int(+1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(0, +1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(-1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(0, -1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
    }
}
