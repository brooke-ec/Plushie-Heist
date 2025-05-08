using UnityEngine;

[CreateAssetMenu(fileName = "New ability skill", menuName = "Scriptable Objects/Skill tree/Skills/New ability skill")]
public class AbilitySkill : Skill
{
    //Something like unlocking an ability, like wall running or grapple

    public AbilityToUnlock abilityToUnlock;
    public enum AbilityToUnlock
    {
        none,
        PlayerDash,
        PlayerBoost,
        PlayerWallRunning,
        PlayerGrapple,
        PlayerGlide,
        PlayerSecondChance,
        ShopAutomaticItemRestocking,
    }

    public override void Unlock()
    {
        bool isNight = NightManager.instance != null;

        switch (abilityToUnlock)
        {
            PlayerController controller = FindAnyObjectByType<PlayerController>();
            case AbilityToUnlock.PlayerDash:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Dash); }
                break;
            case AbilityToUnlock.PlayerBoost:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Boost); }
                break;
            case AbilityToUnlock.PlayerWallRunning:
                controller.wallRunEnabled = true;
                break;
            case AbilityToUnlock.PlayerGrapple:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Grapple); }
                break;
            case AbilityToUnlock.PlayerGlide:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Glide); }
                break;
            case AbilityToUnlock.PlayerSecondChance:
                controller.secondChance = true;
                break;
            case AbilityToUnlock.ShopAutomaticItemRestocking:
                if (!isNight) { }
                //TO-DO AFTER PICKING UP ITEM, IF VARIABLE IS TRUE
                //then checking
                //basically IF VARIABLE IS TRUE
                //bool removed = FindAnyObjectByType<InventoryController>().RemoveAnItemTypeFromInventory(itemClassPassed)
                //place item again in the same place/shelf
                break;
            default:
                break;
        }
    }

    #region Ability upgrade methods
    #region Player
    public void UnlockWallRunning()
    {
        //Something like FindAnyObjectByType<PlayerController>().UnlockWallRunning(); ?
    }

    public void UnlockGrapple()
    {

    }
    #endregion

    #region Shop
    #endregion

    #endregion
}
