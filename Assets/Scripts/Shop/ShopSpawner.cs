using Newtonsoft.Json;
using System.Runtime.Serialization;
using UnityEngine;

public class ShopSpawner : MonoBehaviour
{
    [SerializeField] [JsonProperty] private int level = 0;
    [SerializeField] private GridSaver[] levels;
    
    [SerializeField] [JsonProperty("layout")] private GridSaver current;
    [SerializeField] [JsonProperty("storage")] private InventoryGrid storage;

    static ShopSpawner instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [OnDeserializing]
    internal void Factory(StreamingContext context)
    {
        instance.SetLevel(level);
    }

    private void SetLevel(int level)
    {
        if (current != null) Destroy(current);
        
        current = Instantiate(levels[level], transform);
        this.level = level;
    }
}
