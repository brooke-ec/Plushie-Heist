using System.Linq;
using TNRD;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile List", menuName = "Scriptable Objects/Tile List", order = 9999)]
public class TileList : ScriptableObject, ITileIdentity
{
    [SerializeField] private SerializableInterface<ITileIdentity>[] tiles;

    public LevelTile[] identity => tiles.SelectMany(l => l.Value.identity).ToArray();
}
