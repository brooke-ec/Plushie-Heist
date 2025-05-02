using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(24, 24);
    public RoomGenerator roomGenerator;
    public PathGenerator pathGenerator;
    public TileGenerator tileGenerator;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 physicalSize = new Vector3(size.x - 1, 0, size.y - 1) * tileGenerator.tileSize;
        Gizmos.DrawWireCube(transform.position, physicalSize);
    }
#endif

    private void Start()
    {
        LevelRoom[] rooms = roomGenerator.Generate(size);
        RoomEdge[] edges = pathGenerator.Generate(rooms);

        foreach (LevelRoom room in rooms) room.Shrink(1);

        Region[] regions = rooms.Where(r => r.pathDistance != -1).Cast<Region>()
            .Concat(edges.Where(e => e.connected).Cast<Region>()).ToArray();

        LevelTile[] tiles = tileGenerator.Generate(regions, transform, size);

        bool[,] spaces = new bool[size.x, size.y];
        regions.ForEach(r => r.ForEach(p => spaces[p.x, p.y] = true));
        GetComponent<TileCuller>().Setup(spaces, tiles);

        GetComponent<NavMeshSurface>().BuildNavMesh();
        GetComponent<GaurdSpawer>().SpawnGaurds();

        StaticBatchingUtility.Combine(gameObject);
    }
}
