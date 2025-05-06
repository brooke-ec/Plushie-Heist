using UnityEngine;

public class ShopSpawner : MonoBehaviour, ISavableMap
{
    [SerializeField] private GridSaver[] levels;
    private GridSaver current;

    [Header("Testing")]
    [SerializeField] private int level = 0;

    string ISavable.key => "shop";

    private void Start()
    {
        SetLevel(level);
    }

    private void SetLevel(int level)
    {
        if (current != null) Destroy(current);

        current = Instantiate(levels[level], transform);
    }

    ISavable[] ISavableMap.Collect()
    {
        return new ISavable[] { current };
    }
}
