using Newtonsoft.Json;
using UnityEngine;

public class GridSaver : MonoBehaviour, IMapCollection
{
    [SerializeField] [JsonProperty("grids"), Populate] FurnitureGrid[] grids;
    
    string IMapCollection.key => "grids";
}
