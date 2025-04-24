using UnityEngine;

public class FurnitureSettings : ScriptableObject
{
    public static FurnitureSettings instance => _instance == null ? (_instance = Resources.Load<FurnitureSettings>("Furniture Settings")) : _instance;
    private static FurnitureSettings _instance;

    [field: SerializeField] public Material invalidMaterial { get; private set; }
    [field: Min(0.5f)] [field: SerializeField] public float cellSize { get; private set; } = 1;
    [field: Range(0, 1)] [field: SerializeField] public float spacing { get; private set; } = 0.25f;
}
