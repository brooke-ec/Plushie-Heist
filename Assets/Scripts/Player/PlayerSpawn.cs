using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        PlayerController.instance.MoveSpawnpoint(transform.position);
    }

    private void Awake()
    {
        transform.parent = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "person");
    }
#endif
}
