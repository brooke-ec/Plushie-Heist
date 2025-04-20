using UnityEngine;

public class PlayerAbilityPickup : MonoBehaviour
{
    /// <summary>The name of the function that gets called on the Player</summary>
    [SerializeField] private string functionName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.SendMessage(functionName);
        }
    }
}
