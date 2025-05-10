using UnityEngine;

public class PlushieEnable : MonoBehaviour
{
    [SerializeField] public PlushieInfo plushie;
    [SerializeField] bool inverted = false;

    protected virtual void Start()
    {
        gameObject.SetActive(plushie.unlocked == !inverted);
    }
}
