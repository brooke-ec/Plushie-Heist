using UnityEngine;

[CreateAssetMenu(fileName = "New boost skill", menuName = "Scriptable Objects/Skill tree/Skills/New boost skill")]
public class BoostSkill : Skill
{
    //Something like a stat modifier, such as for movespeed or dash
    public SkillType skillType;
    public float modifier;
    public enum SkillType
    {
        none,
        PlayerBackpackSize,
        PlayerItemLostPercent,
        PlayerExtraTime,
        PlayerExtraDash,
        PlayerExtraJump,
        PlayerExtraBoost,
        ShopExtraTime,
        ShopInventorySize,
        ShopCustomerSpawnRate,
        ShopCustomerTips,
        ShopHigherPrices,
        ShopImpulseBuyers,
        ShopMarketStability,
        ShopExpansion
    }

    public override void Unlock()
    {
        switch (skillType)
        {
            case SkillType.PlayerBackpackSize:
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
