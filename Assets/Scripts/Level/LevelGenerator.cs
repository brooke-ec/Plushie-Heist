using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int size = new(24, 24);
    [SerializeField] public RoomGenerator roomGenerator;
    [SerializeField] public PathGenerator pathGenerator;
    [SerializeField] public TileGenerator tileGenerator;

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

        Region[] spaces = rooms.Where(r => r.pathDistance != -1).Cast<Region>()
            .Concat(edges.Where(e => e.connected).Cast<Region>()).ToArray();

        LevelTile[] tiles = tileGenerator.Generate(spaces, transform, size);
        
        bool[,] gaps = new bool[size.x, size.y];
        tiles.ForEach(t => gaps[t.position.x, t.position.y] = true);
        StaticBatchingUtility.Combine(gameObject);
        FindObjectsOfType<ObjectSpawner>().ForEach(s => s.Spawn());

        GetComponent<TileCuller>().Setup(gaps, tiles);

        GetComponent<NavMeshSurface>().BuildNavMesh();
        GetComponent<GaurdSpawer>().SpawnGaurds();
    }
}
