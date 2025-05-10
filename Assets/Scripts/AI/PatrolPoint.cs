using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    private void Start()
    {
        transform.parent = null;
    }

#if UNITY_EDITOR
    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
#endif
}
