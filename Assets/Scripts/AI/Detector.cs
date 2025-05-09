using UnityEngine;

public class Detector : MonoBehaviour
{
    private GuardAI guardAI;

    private void Start()
    {
        guardAI = GetComponentInParent<GuardAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            guardAI.Detect(other.gameObject);
        }
    }
}
