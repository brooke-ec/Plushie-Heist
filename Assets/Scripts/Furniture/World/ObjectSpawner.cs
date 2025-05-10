using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private SpawnablePropList Props;
    
    private void Start()
    {
        if (Props.Props.Length < 1) { return; }
        int propNo = Random.Range(0, Props.Props.Length);
        if (Props.Props[propNo] == null) { return; }
        Instantiate(Props.Props[propNo], GetComponentInParent<Transform>());
    }

    private void OnDrawGizmos()
    {
        GameObject currentBiggest = Props.Props[0];
        foreach (GameObject i in Props.Props) 
        {
            if (i != null)
            {
                if (currentBiggest == null)
                {
                    currentBiggest = i;
                }
                else if (i.GetComponent<MeshFilter>().sharedMesh.bounds.size.magnitude > currentBiggest.GetComponent<MeshFilter>().sharedMesh.bounds.size.magnitude)
                {
                    currentBiggest = i;
                }
            }
        }
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 pos = transform.position+currentBiggest.transform.position;
        Mesh mesh = currentBiggest.GetComponent<MeshFilter>().sharedMesh;
        mesh.RecalculateNormals();
        Gizmos.DrawMesh(mesh,pos,transform.rotation);
    }
}
