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
        bool isNight = NightManager.instance != null;

        switch (skillType)
        {
            case SkillType.PlayerBackpackSize:
                FindAnyObjectByType<InventoryController>().backpackGrid.ModifyInventorySize((int)modifier);
                break;
            case SkillType.PlayerItemLostPercent:
                if (isNight) { NightManager.instance.itemLosePercentage -= (int)modifier; }
                break;
            case SkillType.PlayerExtraTime:
                if (isNight) { NightManager.instance.UpdateClockTime(modifier); }
                break;
            case SkillType.PlayerExtraDash:
                if (isNight) { FindAnyObjectByType<PlayerController>().ModifyAbilityValue("dash", modifier); }
                break;
            case SkillType.PlayerExtraJump:
                PlayerController playerController = FindAnyObjectByType<PlayerController>();
                if (playerController != null) { playerController.ModifyAbilityValue("jump", modifier); }
                break;
            case SkillType.PlayerExtraBoost:
                if (isNight) { FindAnyObjectByType<PlayerController>().ModifyAbilityValue("boost", modifier); }
                break;
            case SkillType.ShopExtraTime:
                if (!isNight) { ShopManager.instance.UpdateClockTime(modifier); }
                break;
            case SkillType.ShopInventorySize:
                //Something like
                if (!isNight) { FindAnyObjectByType<InventoryController>().backpackGrid.ModifyInventorySize((int)modifier);}
                break;
            case SkillType.ShopCustomerSpawnRate:
                if (!isNight) { } //TO-DO * CUSTOMER SPAWN RATE
                break;
            case SkillType.ShopCustomerTips:
                if (!isNight) { } //TO-DO * AVERAGE TIPS (have a tips variable set to 1)
                break;
            case SkillType.ShopHigherPrices:
                if (!isNight) { } //TO-DO * AVERAGE PRICE RANGE THAT CUSTOMERS WILL BUY
                break;
            case SkillType.ShopImpulseBuyers:
                if (!isNight) { } //TO-DO * AVERAGE NUM OF ITEMS THAT CUSTOMERS BUY
                break;
            case SkillType.ShopMarketStability:
                if (!isNight) {
                    ShopManager.instance.stocksController.maxPercentOfItemsToChange -= 0.2f;
                    ShopManager.instance.stocksController.minPercentOfItemsToChange -= 0.2f;
                }
                break;
            case SkillType.ShopExpansion:
                if (!isNight) { } //TO-DO-SAVING
                break;
            default:
                Debug.Log("Error in boost skill type");
                break;
        }
    }
}
