using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private Dictionary<Vector2Int, LevelTile> tiles;

    [SerializeField] private LevelTile StartTile;
    [SerializeField] private int min = 3;
    [SerializeField] private int max = 8;

    private void Start()
    {
        tiles = new();

        int hmax = max / 2;
        GenerateRooms(new Region(
            Random.Range(-hmax, 0), 0,
            Random.Range(0, hmax), Random.Range(min, max)
        ), 4);

        Display();
    }

    private void GenerateRooms(Region r, int depth)
    {
        for (int x = r.sx; x <= r.ex; x++)
            for (int y = r.sy; y <= r.ey; y++)
                tiles.Add(new Vector2Int(x, y), null);

        if (depth == 0) return;

        while (true) {
            Vector2Int door;
            Vector2Int direction;
            int choice = Random.Range(0, 4);
            if (choice == 0) { direction = new Vector2Int(0, 1); door = new Vector2Int(Random.Range(r.sx, r.ex + 1), r.ey + 1); }
            else if (choice == 1) { direction = new Vector2Int(1, 0); door = new Vector2Int(r.ex + 1, Random.Range(r.sy, r.ey + 1)); }
            else if (choice == 2) { direction = new Vector2Int(0, -1); door = new Vector2Int(Random.Range(r.sx, r.ex + 1), r.sy - 1); }
            else if (choice == 3) { direction = new Vector2Int(-1, 0); door = new Vector2Int(r.sx - 1, Random.Range(r.sy, r.ey + 1)); }
            else throw new System.Exception("Invalid choice while generating level");
            Vector2Int transformation = new Vector2Int(direction.x == 0 ? 1 : direction.x, direction.y == 0 ? 1 : direction.y);
            Vector2Int position = door + direction;

            Vector2Int end = position + new Vector2Int(Random.Range(min, max), Random.Range(min, max)) * transformation;

            Region nr = new Region(position.x, position.y, end.x, end.y);

            if (!IsEmpty(nr)) continue;

            tiles.Add(door, null);
            GenerateRooms(nr, depth - 1);
            break;
        }
    }

    private bool IsEmpty(Region r)
    {
        for (int x = r.sx; x <= r.ex; x++)
            for (int y = r.sy; y <= r.ey; y++)
                if (tiles.ContainsKey(new Vector2Int(x, y))) return false;
        return true;
    }

    private void Display()
    {
        //Queue<Vector2Int> next = new Queue<Vector2Int>();
        //Instantiate(StartTile, new Vector3(0, 0, 0), Quaternion.identity, transform);
        //next.Enqueue(new Vector2Int(0, 0));

        //foreach (Vector2Int pos in next)
        //{
        //    List<LevelTile> possible = new List<LevelTile>();
        //    Vector2Int tpos;

        //    tpos = new Vector2Int(pos.x, pos.y + 1);
        //    if (tiles.TryGetValue(pos, out LevelTile positiveZ))
        //    {
        //        if (positiveZ != null) possible.AddRange(positiveZ.NegativeZ);
        //        if (!next.Contains(tpos)) next.Enqueue(tpos);
        //    }

        //    tpos = new Vector2Int(pos.x + 1, pos.y);
        //    if (tiles.TryGetValue(pos, out LevelTile positiveX))
        //    {
        //        if (positiveZ != null) possible.AddRange(positiveX.NegativeX);
        //        if (!next.Contains(tpos)) next.Enqueue(tpos);
        //    }

        //    tpos = new Vector2Int(pos.x, pos.y - 1);
        //    if (tiles.TryGetValue(pos, out LevelTile negativeZ))
        //    {
        //        if (positiveZ != null) possible.AddRange(negativeZ.PositiveZ);
        //        if (!next.Contains(tpos)) next.Enqueue(tpos);
        //    }

        //    tpos = new Vector2Int(pos.x - 1, pos.y);
        //    if (tiles.TryGetValue(pos, out LevelTile negativeX))
        //    {
        //        if (positiveZ != null) possible.AddRange(negativeX.PositiveX);
        //        if (!next.Contains(tpos)) next.Enqueue(tpos);
        //    }

        //    possible = possible.Where((LevelTile t) =>
        //        t.PositiveZ.Contains(positiveZ)
        //        && t.PositiveX.Contains(positiveX)
        //        && t.NegativeZ.Contains(negativeZ)
        //        && t.PositiveX.Contains(positiveX)
        //    ).ToList();

        //    Instantiate(possible[Random.Range(0, possible.Count)], new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
        //}

        foreach (Vector2Int pos in tiles.Keys)
            Instantiate(StartTile, new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
    }

    private struct Region
    {
        public int sx;
        public int sy;
        public int ex;
        public int ey;

        public Region(int sx, int sy, int ex, int ey)
        {
            this.sx = Mathf.Min(sx, ex);
            this.sy = Mathf.Min(sy, ey);
            this.ex = Mathf.Max(sx, ex);
            this.ey = Mathf.Max(sy, ey);
        }
    }
}
