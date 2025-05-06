using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture Item", menuName ="Scriptable Objects/Furniture Item", order = 9999)]
public class FurnitureItem : ScriptableObject
{
    public const string ASSET_PATH = "Assets/Resources/Items/";

    /// <summary> The name of this item displayed on the stock UI </summary>
    public string itemName;
    /// <summary> The prefab representing this item </summary>
    public FurnitureController prefab;
    /// <summary> The base value of this item </summary>
    [field: SerializeField] public int marketPrice { get; private set; }

    [field: Header("Placement Settings")]
    /// <summary> The size of this item in on the <see cref="FurnitureGrid"/> </summary>
    [field: SerializeField] public Vector2Int gridSize { get; private set; }
    /// <summary> The offset from default when placing on a <see cref="FurnitureGrid"/> </summary>
    [field: SerializeField] public Vector3 gridOffset { get; private set; }

    [field: Header("Inventory Settings")]
    /// <summary> The size of this item in the inventory </summary>
    [field: SerializeField] public Vector2Int inventorySize { get; private set; }
    /// <summary> The icon to use for this item in the inventory </summary>
    [field: SerializeField] public Sprite inventoryIcon { get; private set; }
}
