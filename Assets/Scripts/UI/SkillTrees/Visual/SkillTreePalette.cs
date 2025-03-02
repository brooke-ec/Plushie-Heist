using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill tree palette", menuName = "Scriptable Objects/Skill tree/New skill tree palette")]
public class SkillTreePalette : ScriptableObject
{
    public Color32 unlockedColour = Color.white;
    public Color32 unlockedLineColour = Color.white;
    public Sprite unlockedSprite;

    public Sprite canBeUpgradedSprite;
    public Sprite lockedSprite;

    public Color32 greyedOut = new Color32(88, 88, 88, 112);

    public Color32 upgradedTextColour = Color.white;
    public Color32 notUpgradedTextColour = Color.white;

    public HoveringManager.TooltipBackgroundColor tooltipBackgroundColor;
}
