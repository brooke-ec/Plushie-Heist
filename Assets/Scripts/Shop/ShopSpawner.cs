using UnityEngine;

public class ShopSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] levels;
    private GameObject current;

    [Header("Testing")]
    [SerializeField] private int level = 0;

    private void Start()
    {
        SetLevel(level);
    }

    private void SetLevel(int level)
    {
        if (current != null) Destroy(current);

        current = Instantiate(levels[level], transform);
    }
}
