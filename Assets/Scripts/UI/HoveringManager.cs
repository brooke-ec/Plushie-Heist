using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoveringManager : MonoBehaviour
{
    public GameObject tooltipPrefab;

    public static GameObject currentTooltipOpen;
    private Transform canvasTransform;
    public Vector3 offset = Vector3.one;

    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Color32 coinTextColour;

    [SerializeField] private Sprite blueTooltipBackground;
    [SerializeField] private Sprite greyTooltipBackground;
    [SerializeField] private Sprite pinkTooltipBackground;

    /// <summary> So that some things can cost stars, money, etc, depending on what we want </summary>
    public enum TooltipCost
    {
        none,
        coins
    }

    public enum TooltipBackgroundColor
    {
        noChanges,
        blue,
        grey,
        pink
    }

    private void Start()
    {
        canvasTransform = FindAnyObjectByType<UIManager>().rootCanvas.transform;
        CalculateOffset();
    }

    private void CalculateOffset()
    {
        offset = new Vector3((Screen.width / offset.x), (Screen.height / offset.y), 0);
    }

    public void CreateBaseTooltip(string title, Color32 titleColour, string description, Vector3 screenPosition, TooltipCost tooltipCost = TooltipCost.none, string tooltipCostText = null, TooltipBackgroundColor tooltipColour = TooltipBackgroundColor.noChanges)
    {
        currentTooltipOpen = Instantiate(tooltipPrefab, canvasTransform);
        currentTooltipOpen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        currentTooltipOpen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = titleColour;
        currentTooltipOpen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;

        if (tooltipCost.Equals(TooltipCost.none) || tooltipCostText == null)
        {
            currentTooltipOpen.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            currentTooltipOpen.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tooltipCostText;

            //do colours and icons
            switch (tooltipCost)
            {
                case TooltipCost.coins:
                    currentTooltipOpen.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().color = coinTextColour;
                    currentTooltipOpen.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = coinIcon;
                    break;
                default:
                    break;
            }
        }

        currentTooltipOpen.transform.position = screenPosition - offset;

        switch (tooltipColour)
        {
            case TooltipBackgroundColor.blue:
                currentTooltipOpen.transform.GetComponent<Image>().sprite = blueTooltipBackground;
                break;
            case TooltipBackgroundColor.grey:
                currentTooltipOpen.transform.GetComponent<Image>().sprite = greyTooltipBackground;
                break;
            case TooltipBackgroundColor.pink:
                currentTooltipOpen.transform.GetComponent<Image>().sprite = pinkTooltipBackground;
                break;
            default:
                break;
        }
    }
}
