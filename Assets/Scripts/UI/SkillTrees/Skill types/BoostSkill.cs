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
                if (!SaveManager.deserializing) FindAnyObjectByType<InventoryController>().backpackGrid.ModifyInventorySize((int)modifier);
                break;
            case SkillType.PlayerItemLostPercent:
                if (isNight) { NightManager.instance.itemLosePercentage -= (int)modifier; }
                break;
            case SkillType.PlayerExtraTime:
                if (isNight) { NightManager.instance.UpdateClockTime(modifier); }
                break;
            case SkillType.PlayerExtraDash:
                if (isNight) { PlayerController.instance.ModifyAbilityValue("dash", modifier); }
                break;
            case SkillType.PlayerExtraJump:
                PlayerController.instance.ModifyAbilityValue("jump", modifier);
                break;
            case SkillType.PlayerExtraBoost:
                if (isNight) { PlayerController.instance.ModifyAbilityValue("boost", modifier); }
                break;
            case SkillType.ShopExtraTime:
                if (!isNight) { ShopManager.instance.UpdateClockTime(modifier); }
                break;
            case SkillType.ShopInventorySize:
                if (!isNight && !SaveManager.deserializing) { InventoryController.instance.storageGrid.ModifyInventorySize((int)modifier);}
                break;
            case SkillType.ShopCustomerSpawnRate:
                if (!isNight) {
                        CustomerController customerController = FindAnyObjectByType<CustomerController>();
                        if(customerController!=null) { customerController.IncreaseCustomerSpawnRate(modifier); }
                    }
                break;
            case SkillType.ShopCustomerTips:
                if (!isNight) { Debug.Log("Unlock"); ShopManager.instance.tipPercentage += modifier; }
                break;
            case SkillType.ShopHigherPrices:
                if (!isNight) ShopManager.instance.stocksController.purchaseRange += Vector2.one * 0.05f;
                break;
            case SkillType.ShopImpulseBuyers:
                if (!isNight) { ShopManager.instance.itemBuyingMultiplier += modifier; }
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
