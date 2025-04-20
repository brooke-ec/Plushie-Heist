using UnityEngine;

public class Hook : MonoBehaviour
{
    ///<summary>The Player</summary>
    private GameObject player;
    /// <summary>The Rope</summary>
    [SerializeField] private GameObject Rope;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player/GrappleAttachPoint");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 line = this.transform.position - player.transform.position;
        Rope.transform.localScale = new Vector3(0.2f, line.magnitude * 2.5f, 0.2f);
        Rope.transform.position = this.transform.position - line / 2;
        Rope.transform.LookAt(player.transform);
        Rope.transform.Rotate(new Vector3(90, 0, 0));
    }

    private void KillHook()
    {
        Destroy(gameObject);
    }
}
