using UnityEngine;

public class PlushieEnable : MonoBehaviour
{
    [SerializeField] PlushieInfo plushie;

    void Start()
    {
        gameObject.SetActive(plushie.unlocked);
    }
}
