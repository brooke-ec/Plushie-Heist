using Newtonsoft.Json;
using UnityEngine;

public class ShopLayout : MonoBehaviour
{
    [JsonProperty("grids"), Populate] public FurnitureGrid[] grids;
    [JsonProperty("level")] [HideInInspector] public int level;

    [DeserializationFactory]
    public static ShopLayout Factory(int level)
    {
        return ShopManager.instance.SetLayout(level);
    }
}
