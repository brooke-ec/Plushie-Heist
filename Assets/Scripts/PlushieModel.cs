using UnityEngine;

public class PlushieModel : MonoBehaviour
{
    [SerializeField] private bool nextPlushie = true;

    void Start()
    {
        PlushieInfo prefab = SharedUIManager.instance.plushie;
        if (nextPlushie) prefab = prefab.next;
        Instantiate(prefab.prefab, transform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "plushie");
    }
#endif
}
