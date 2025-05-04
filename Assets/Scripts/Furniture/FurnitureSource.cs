using cakeslice;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureSource : MonoBehaviour, IInteractable
{
    /// <summary> The item this prefab represents </summary>
    public FurnitureItem item = null;
    /// <summary> Prompt Shown by the UI to let the player know they can interact with this </summary>
    string IInteractable.interactionPrompt => inventoryHasSpace ? "Press F to Pickup" : "Inventory Full";
    /// <summary> If there is enough space in the players inventory for this item </summary>
    public bool inventoryHasSpace { get; private set; } = true;
    /// <summary> Whether this the interable outline should be red </summary>
    bool IInteractable.outline => inventoryHasSpace;

    private void Start()
    {
        if (InventoryController.instance != null) InventoryController.instance.onChanged.AddListener(() => {
            inventoryHasSpace = InventoryController.instance.CanInsert(item);
        });

        GetComponentsInChildren<MeshRenderer>().ForEach(r => r.GetOrAddComponent<Outline>().enabled = false);
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    /// <returns>True if picked up item<returns>
    public void PrimaryInteract(Interactor interactor)
    {
        if (InventoryController.instance.InsertItem(item))
        {
            Destroy(gameObject);
            Debug.Log("Picked Up" + gameObject.name);
        }
        else Debug.Log("Can't Pick up" + gameObject.name);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (item == null || stage == null || stage.prefabContentsRoot != gameObject) return;

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + item.gridOffset, new Vector3(item.gridSize.x, 0, item.gridSize.y) * FurnitureSettings.instance.cellSize);
    }
#endif
}
