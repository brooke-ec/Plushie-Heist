using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureItem : PickUpInteraction, IInteractable
{
    [field: SerializeReference] public Vector2Int size { get; private set; }

    public bool empty => subgrids.All(s => s.IsEmpty());
    public override bool canPickup => base.canPickup && empty;
    public override string interactionPrompt => empty ? base.interactionPrompt : "Item Contains Sub-Items";

    public Vector2Int position { get; private set; }
    public int rotation { get; private set; } = 0;
    public FurnitureGrid owner { get; set; }
    
    public FurnitureGrid[] subgrids { get; private set; }

    public Region region => new Region().FromSize(position, size);

    private Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();
    private Material previousMaterial = null;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(size.x, 0, size.y) * FurnitureSettings.instance.cellSize);
    }
#endif

    protected override void Start()
    {
        subgrids = GetComponentsInChildren<FurnitureGrid>();
        base.Start();
    }

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
        transform.Rotate(0, 90, 0);
    }

    public void Move(Vector2Int target)
    {
        position = Util.Clamp(target, Vector2Int.zero, owner.size - size);

        transform.position = owner.ToWorldspace(region.center);
        SetMaterial(IsValid() ? null : FurnitureSettings.instance.invalidMaterial);
    }

    public bool IsValid()
    {
        return owner != null && position != null && region.Within(owner.size) && !owner.Intersects(region);
    }

    public void SetMaterial(Material material)
    {
        if (material == previousMaterial) return;

        foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
        {
            if (previousMaterial == null) originalMaterials[renderer.GetInstanceID()] = renderer.material;

            if (material == null && originalMaterials.TryGetValue(renderer.GetInstanceID(), out Material original))
                renderer.material = original;
            else
                renderer.material = material;
        }
        
        previousMaterial = material;
    }

    public override bool Interact(Interactor interactor)
    {
        bool success = base.Interact(interactor);
        if (success) owner.RemoveItem(this);
        return success;
    }
}
