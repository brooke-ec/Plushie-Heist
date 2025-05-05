using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private Transform[] objects;
    [SerializeField] private Vector3 destination;
    [SerializeField] private Vector2 spawnDelay;
    [SerializeField] private float speed;

    private List<Transform> managed = new List<Transform>();
    private float timer = 0;

    private void Start()
    {
        // Prefill path with objects
        this.RunAfter(0, () =>
        {
            int iterations = Mathf.RoundToInt(destination.magnitude / (speed * Time.deltaTime));
            for (int i = 0; i < iterations; i++) Update();
        });
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = Random.Range(spawnDelay.x, spawnDelay.y);
            managed.Add(Instantiate(objects[Random.Range(0, objects.Length)], transform));
        }

        for (int i = 0;  i < managed.Count; i++)
        {
            Transform t = managed[i];
            t.localPosition += destination.normalized * speed * Time.deltaTime;
            if (t.localPosition.magnitude >= destination.magnitude)
            {
                Destroy(t.gameObject);
                managed.RemoveAt(i);
                i--;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(destination));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(destination), 0.2f);
    }
#endif
}
