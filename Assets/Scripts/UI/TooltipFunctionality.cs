using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> Add this to any gameobject that you want to display a tooltip for </summary>
public class TooltipFunctionality : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<TooltipFunctionality> GetTooltipInfo;

    float timeSinceMouseOver = 0;
    private bool mouseOver = false;

    [Header("Extra info")]


    [Header("YOU DON'T HAVE TO DO THESE MANUALLY, but you can")]
    public string title;
    public Color32 titleColour;
    public string description;
    public HoveringManager.TooltipCost tooltipCostType = HoveringManager.TooltipCost.none;
    public HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges;
    public string tooltipCostText = null;
    public string tooltipLockedText = null;

    public void SetInfo(string title, Color32 titleColour, string description, HoveringManager.TooltipCost tooltipCostType, string tooltipCostText, string tooltipLockedText, HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges)
    {
        this.title = title;
        this.titleColour = titleColour;
        this.description = description;
        this.tooltipCostType = tooltipCostType;
        this.tooltipCostText = tooltipCostText;
        this.tooltipLockedText = tooltipLockedText;
        this.tooltipBackgroundColor = tooltipBackgroundColor;
    }

    IEnumerator CheckIfMouseStillOver()
    {
        while (mouseOver)
        {
            timeSinceMouseOver += 1 * Time.deltaTime;
            //if enough time has passed and there isn't already another tooltip open
            if ((timeSinceMouseOver / 40f) >= 0.4f && HoveringManager.currentTooltipOpen == null)
            {
                //you could check here for type of tooltip to create another tooltip type
                FindAnyObjectByType<HoveringManager>().CreateBaseTooltip(title, titleColour, description, transform.position, tooltipCostType, tooltipCostText, tooltipLockedText, tooltipBackgroundColor);
            }
            else
            {
                timeSinceMouseOver++;
            }
            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick2);
        mouseOver = true;
        timeSinceMouseOver = 0;
        GetTooltipInfo?.Invoke(this);
        StartCoroutine(CheckIfMouseStillOver());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        timeSinceMouseOver = 0;
        StopCoroutine(CheckIfMouseStillOver());
        Destroy(HoveringManager.currentTooltipOpen);
    }
}
