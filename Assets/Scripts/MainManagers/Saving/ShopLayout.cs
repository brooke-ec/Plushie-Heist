using Newtonsoft.Json;
using UnityEngine;

public class ShopLayout : MonoBehaviour
{
    [SerializeField] [JsonProperty("grids"), Populate] private FurnitureGrid[] grids;
    [JsonProperty("level")] public int level;

    [DeserializationFactory]
    public static ShopLayout Factory(int level)
    {
        return ShopManager.instance.SetLayout(level);
    }
}
