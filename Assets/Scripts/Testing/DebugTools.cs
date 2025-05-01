using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private bool hideFurniture;
    private bool furnitureHidden = false;

    [SerializeField] private bool disableGaurds;
    private bool guardsDisabled = false;

    [SerializeField] private int cameraCount;

    private void Update()
    {
        if (furnitureHidden != hideFurniture)
        {
            furnitureHidden = hideFurniture;
            FindObjectsOfType<FurnitureItem>().ForEach(i => i.gameObject.SetActive(!furnitureHidden));
        }

        if (guardsDisabled != disableGaurds)
        {
            guardsDisabled = disableGaurds;
            FindObjectsOfType<GaurdAI>().ForEach(g => g.gameObject.SetActive(!guardsDisabled));
        }

        cameraCount = Camera.allCamerasCount;
    }
}
