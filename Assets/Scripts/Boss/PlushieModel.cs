using UnityEngine;

[DefaultExecutionOrder(-10)]
public class PlushieModel : MonoBehaviour
{
    [SerializeField] private bool nextPlushie = true;

    void Start()
    {
        PlushieInfo plushie = SharedUIManager.instance.plushie;
        if (nextPlushie) plushie = PlushieInfo.Next(plushie);
        if (plushie == null) Destroy(gameObject);
        else Instantiate(plushie.prefab, transform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "plushie");
    }
#endif
}
