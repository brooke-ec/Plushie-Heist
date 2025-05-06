using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntranceDoor : MonoBehaviour
{
    [SerializeField] private float activationRadius = 1.0f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float speed = 2;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip openSound;

    private Collider[] colliders = new Collider[1];
    private new AudioSource audio;
    private bool closed = true;
    private int count = 0;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, activationRadius, colliders, layerMask);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            count == 0 ? Quaternion.identity : Quaternion.Euler(0, 90, 0),
            Time.deltaTime * speed
        );

        // Check if closed using angle
        bool nowClosed = Mathf.Abs(transform.eulerAngles.y) < 20;
        if (nowClosed != closed)
        {
            closed = nowClosed;
            audio.clip = closed ? closeSound : openSound;
            audio.Play();
        }
    }

#if UNITY_EDITOR
    Mesh mesh;

    private void OnValidate()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        Gizmos.DrawMesh(mesh, transform.position, Quaternion.Euler(0, 90, 0), transform.lossyScale);
    }
#endif
}
