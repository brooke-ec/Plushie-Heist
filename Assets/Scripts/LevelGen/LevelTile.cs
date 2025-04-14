using UnityEditor;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public LevelTile[] positiveX;
    public LevelTile[] negativeX;
    public LevelTile[] positiveZ;
    public LevelTile[] negativeZ;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (!Application.isPlaying) Gizmos.DrawWireCube(transform.position, new Vector3(10, 0, 10));
    }
#endif
}
