using System.Linq;
using TNRD;
using UnityEngine;

public class LevelTile : MonoBehaviour, ITileIdentity
{
    [SerializeField] private SerializableInterface<ITileIdentity>[] _positiveX;
    [SerializeField] private SerializableInterface<ITileIdentity>[] _positiveZ;
    [SerializeField] private SerializableInterface<ITileIdentity>[] _negativeX;
    [SerializeField] private SerializableInterface<ITileIdentity>[] _negativeZ;
    [Tooltip("Exclude this tile from spawning naturally")] public bool exclude = false;

    [HideInInspector] public LevelTile[] positiveX => _positiveX.SelectMany(l => l.Value.identity).ToArray();
    [HideInInspector] public LevelTile[] positiveZ => _positiveZ.SelectMany(l => l.Value.identity).ToArray();
    [HideInInspector] public LevelTile[] negativeX => _negativeX.SelectMany(l => l.Value.identity).ToArray();
    [HideInInspector] public LevelTile[] negativeZ => _negativeZ.SelectMany(l => l.Value.identity).ToArray();

    public LevelTile[] identity => new LevelTile[] { this };

    public static string TilesToString(LevelTile[] array)
    {
        return string.Format("[{0}]", string.Join(", ", array.Select(t => "\"" + t.name + "\"")));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (!Application.isPlaying) Gizmos.DrawWireCube(transform.position, new Vector3(10, 0, 10));
    }
#endif
}
