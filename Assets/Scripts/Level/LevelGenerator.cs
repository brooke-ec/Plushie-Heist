using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int size = new(24, 24);
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private PathGenerator pathGenerator;
    [SerializeField] private TileGenerator tileGenerator;

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

        tileGenerator.Generate(spaces, transform, size);
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
