using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> Add this to any gameobject that you want to display a tooltip for </summary>
public class TooltipFunctionality : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<TooltipFunctionality> GetTooltipInfo;

    float framesPassedSinceOver = 0;
    private bool mouseOver = false;

    [Header("Extra info")]


    [Header("YOU DON'T HAVE TO DO THESE MANUALLY, but you can")]
    public string title;
    public Color32 titleColour;
    public string description;
    public HoveringManager.TooltipCost tooltipCostType = HoveringManager.TooltipCost.none;
    public HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges;
    public string tooltipCostText = null;

    public void SetInfo(string title, Color32 titleColour, string description, HoveringManager.TooltipCost tooltipCostType, string tooltipCostText, HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges)
    {
        this.title = title;
        this.titleColour = titleColour;
        this.description = description;
        this.tooltipCostType = tooltipCostType;
        this.tooltipCostText = tooltipCostText;
        this.tooltipBackgroundColor = tooltipBackgroundColor;
    }

    IEnumerator CheckIfMouseStillOver()
    {
        while (mouseOver)
        {
            framesPassedSinceOver += 1 * Time.deltaTime;
            //if enough time has passed and there isn't already another tooltip open
            if ((framesPassedSinceOver / 40f) >= 2 && HoveringManager.currentTooltipOpen == null)
            {
                //you could check here for type of tooltip to create another tooltip type
                FindAnyObjectByType<HoveringManager>().CreateBaseTooltip(title, titleColour, description, transform.position, tooltipCostType, tooltipCostText, tooltipBackgroundColor);
            }
            else
            {
                framesPassedSinceOver++;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        framesPassedSinceOver = 0;
        GetTooltipInfo?.Invoke(this);
        StartCoroutine(CheckIfMouseStillOver());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        framesPassedSinceOver = 0;
        StopCoroutine(CheckIfMouseStillOver());
        Destroy(HoveringManager.currentTooltipOpen);
    }
}
