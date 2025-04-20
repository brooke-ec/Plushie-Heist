using TNRD;
using UnityEngine;

public class LevelTile : MonoBehaviour, ITileList
{
    [SerializeField] public SerializableInterface<ITileList>[] positiveX;
    [SerializeField] public SerializableInterface<ITileList>[] negativeX;
    [SerializeField] public SerializableInterface<ITileList>[] positiveZ;
    [SerializeField] public SerializableInterface<ITileList>[] negativeZ;

    public LevelTile[] identity => new LevelTile[] { this };

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (!Application.isPlaying) Gizmos.DrawWireCube(transform.position, new Vector3(10, 0, 10));
    }
#endif
}
