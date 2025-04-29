using UnityEngine;

[CreateAssetMenu(fileName = "New boost skill", menuName = "Scriptable Objects/Skill tree/Skills/New boost skill")]
public class BoostSkill : Skill
{
    //Something like a stat modifier, such as for movespeed or dash
    public float modifier;
    public SkillType skillType;
    public enum SkillType
    {
        none,
        PlayerPlusOneBackpackSize,
        PlayerPlusTwoBackpackSize,
        PlayerItemLostPercent,
        PlayerExtraTime,
        PlayerExtraDash,
        PlayerExtraJump,
        PlayerExtraBoost,
        ShopExtraTime,
        ShopInventorySize,
    }

    public override void Unlock()
    {
        switch (skillType)
        {
            case SkillType.PlayerPlusOneBackpackSize:
                break;
            case SkillType.ShopInventorySize:
                //Something like
                //FindAnyObjectByType<InventoryController>().selectedInventoryGrid.ModifyInventorySize();
                break;
            default:
                break;
        }
    }
}
