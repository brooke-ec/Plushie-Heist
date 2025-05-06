using UnityEngine;

public class GridSaver : MonoBehaviour, ISavableArray
{
    [SerializeField] FurnitureGrid[] grids = new FurnitureGrid[0];
    
    string ISavable.key => "grids";

    ISavable[] ISavableArray.Collect()
    {
        return grids;
    }
}
