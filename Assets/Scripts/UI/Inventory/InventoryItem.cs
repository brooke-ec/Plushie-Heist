using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    [JsonProperty("item"), Unwitable] public FurnitureItem itemClass;
    [JsonProperty("position")] public Vector2Int position;
    [JsonProperty("rotated")] public bool rotated = false;
    private Image icon;
    private RectTransform shadowRectTransform;
    private RectTransform backgroundRectTransform;

    [DeserializationFactory]
    public static InventoryItem Factory(FurnitureItem item, bool rotated)
    {
        Transform rootCanvas = SharedUIManager.instance.rootCanvas.transform;
        InventoryItem ii = Instantiate(InventoryController.instance.itemPrefab, rootCanvas).GetComponent<InventoryItem>();
        ii.Set(item);
        if (rotated) ii.Rotate();
        return ii;
    }

    public static InventoryItem Factory(FurnitureItem item)
    {
        return Factory(item, false);
    }

    private float scaleFactor;

    private void Awake()
    {
        shadowRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        backgroundRectTransform = transform.GetChild(1).GetComponent<RectTransform>();
        icon = transform.GetChild(2).GetComponent<Image>();

        scaleFactor = FindAnyObjectByType<SharedUIManager>().scaleFactor;
    }

    public int Height
    {
        get
        {
            if (!rotated)
            {
                return itemClass.inventorySize.y;
            }
            return itemClass.inventorySize.x;
        }
    }

    public int Width
    {
        get
        {
            if (!rotated)
            {
                return itemClass.inventorySize.x;
            }
            return itemClass.inventorySize.y;
        }
    }

    /// <summary> Mostly used to pick up an item. Gets the INDEXES of the main position on grid, x and y </summary>
    public Vector2Int mainPositionOnGrid = new Vector2Int();

    public void Set(FurnitureItem itemClass)
    {
        this.itemClass = itemClass;
        icon.sprite = itemClass.inventoryIcon;

        Vector2 size = new Vector2();
        size.x = Width * InventoryGrid.usableTileSize;
        size.y = Height * InventoryGrid.usableTileSize;

        icon.GetComponent<RectTransform>().sizeDelta = size;
        SetBackground(size);
    }

    private void SetBackground(Vector2 size)
    {
        int horizontalOffsetNeeded = (InventoryGrid.offsetFromImage + InventoryGrid.offsetFromShadow) * (Width - 1); //this is so objects of size 1 have no extra-tile spacing offsets
        int verticalOffsetNeeded = (InventoryGrid.offsetFromImage + InventoryGrid.offsetFromShadow) * (Height - 1);
        size = new Vector2(size.x + horizontalOffsetNeeded, size.y + verticalOffsetNeeded);

        shadowRectTransform.sizeDelta = size;
        backgroundRectTransform.sizeDelta = size;
    }

    public void Rotate()
    {
        if (itemClass.inventorySize.x == itemClass.inventorySize.y) { return; }

        rotated = !rotated;
        RectTransform itemRectTransform = GetComponent<RectTransform>();
        itemRectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f);

        //Adjust shadow so it's below
        Vector3 newBackgroundPos = new Vector3(backgroundRectTransform.position.x + InventoryGrid.offsetFromShadow, backgroundRectTransform.position.y - InventoryGrid.offsetFromShadow, 0);
        shadowRectTransform.position = newBackgroundPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //If right click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            print("use");

            AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
            GetComponentInParent<InventoryGrid>().CreateItemInteractionMenu(this);
        }
    }
}
