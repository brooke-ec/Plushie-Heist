using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        PlayerController.instance.transform.position = transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "person");
    }
#endif
}
