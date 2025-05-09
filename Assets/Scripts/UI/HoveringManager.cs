using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HoveringManager : MonoBehaviour
{
    [SerializeField] private Transform tooltipsCanvas;

    public GameObject tooltipPrefab;
    public GameObject inventoryTooltipPrefab;
    public GameObject inventoryTooltipInteractionPrefab;

    public static GameObject currentTooltipOpen;

    public Vector3 offset = Vector3.one;

    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Color32 coinTextColour;

    [SerializeField] private Sprite blueTooltipBackground;
    [SerializeField] private Sprite greyTooltipBackground;
    [SerializeField] private Sprite pinkTooltipBackground;

    [SerializeField] private Sprite unlockedIcon;
    [SerializeField] private Sprite lockedIcon;

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
        CalculateOffset();
        SharedUIManager.instance.onMenuClose.AddListener(() =>
        {
            if (currentTooltipOpen != null) Destroy(currentTooltipOpen);
        });
    }

    private void CalculateOffset()
    {
        offset = new Vector3((Screen.width / offset.x), (Screen.height / offset.y), 0);
    }

    /// <summary>
    /// Call to create an inventory tooltip
    /// </summary>
    /// <param name="actionTitles">Titles of all the interactions</param>
    /// <param name="actions">Actions of all the interactions</param>
    /// <param name="screenPosition">Mouse pos</param>
    public void CreateInventoryTooltip(List<string> actionTitles, List<UnityAction> actions, Vector3 screenPosition)
    {
        if(currentTooltipOpen!=null)
        {
            Destroy(currentTooltipOpen);
        }

        currentTooltipOpen = Instantiate(inventoryTooltipPrefab, tooltipsCanvas);
        currentTooltipOpen.transform.position = screenPosition - offset;

        Transform container = currentTooltipOpen.transform.GetChild(0);

        actionTitles.Add("Close");
        actions.Add(() => Destroy(currentTooltipOpen));
        for (int i=0; i<actionTitles.Count; i++)
        {
            Button action = Instantiate(inventoryTooltipInteractionPrefab, container).GetComponent<Button>();
            action.onClick.AddListener(() => AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick));
            action.onClick.AddListener(actions[i]);
            action.onClick.AddListener(()=>Destroy(currentTooltipOpen));
            action.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionTitles[i];
        }

    }

    public void CreateBaseTooltip(string title, Color32 titleColour, string description, Vector3 screenPosition, TooltipCost tooltipCost = TooltipCost.none, string tooltipCostText = null, string lockedText=null, TooltipBackgroundColor tooltipColour = TooltipBackgroundColor.noChanges)
    {
        currentTooltipOpen = Instantiate(tooltipPrefab, tooltipsCanvas);
        currentTooltipOpen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
        currentTooltipOpen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = titleColour;
        currentTooltipOpen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;

        if (tooltipCost.Equals(TooltipCost.none) || tooltipCostText == null)
        {
            currentTooltipOpen.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            if(lockedText.StartsWith("Locked"))
            {
                currentTooltipOpen.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = lockedIcon;
            }
            else
            {
                currentTooltipOpen.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = unlockedIcon;
            }
            currentTooltipOpen.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = titleColour;
            currentTooltipOpen.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().color = titleColour;
            currentTooltipOpen.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = lockedText;
            currentTooltipOpen.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = tooltipCostText;


            //do colours and icons
            switch (tooltipCost)
            {
                case TooltipCost.coins:
                    currentTooltipOpen.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().color = coinTextColour;
                    currentTooltipOpen.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Image>().sprite = coinIcon;
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
