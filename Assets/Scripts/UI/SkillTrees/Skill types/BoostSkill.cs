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
            case SkillType.PlayerItemLostPercent:
                break;
            case SkillType.PlayerExtraTime:
                break;
            case SkillType.PlayerExtraDash:
                break;
            case SkillType.PlayerExtraJump:
                break;
            case SkillType.PlayerExtraBoost:
                break;
            case SkillType.ShopExtraTime:
                break;
            case SkillType.ShopInventorySize:
                //Something like
                //FindAnyObjectByType<InventoryController>().selectedInventoryGrid.ModifyInventorySize();
                break;
            case SkillType.ShopCustomerSpawnRate:
                break;
            case SkillType.ShopCustomerTips:
                break;
            case SkillType.ShopHigherPrices:
                break;
            case SkillType.ShopImpulseBuyers:
                break;
            case SkillType.ShopMarketStability:
                break;
            case SkillType.ShopExpansion:
                break;
            default:
                Debug.Log("Error in boost skill type");
                break;
        }
    }
}
