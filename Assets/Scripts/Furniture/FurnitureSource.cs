using UnityEngine;

public class FurnitureSource : MonoBehaviour
{
    /// <summary> The item this prefab represents </summary>
    public FurnitureItem item = null;

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
