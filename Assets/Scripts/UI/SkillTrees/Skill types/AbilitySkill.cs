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
        switch (abilityToUnlock)
        {
            case AbilityToUnlock.PlayerDash:
                MovementUIManager.instance.LearnAbility(Ability.Dash);
                break;
            case AbilityToUnlock.PlayerBoost:
                MovementUIManager.instance.LearnAbility(Ability.Boost);
                break;
            case AbilityToUnlock.PlayerWallRunning:
                    //NOT SURE BECAUSE IT'S WALL RUNNING
                    //MovementUIManager.instance.LearnAbility(); ?
                break;
            case AbilityToUnlock.PlayerGrapple:
                MovementUIManager.instance.LearnAbility(Ability.Grapple);
                break;
            case AbilityToUnlock.PlayerGlide:
                MovementUIManager.instance.LearnAbility(Ability.Glide);
                break;
            case AbilityToUnlock.PlayerSecondChance:
                //TO-DO SOME VARIABLE WHEN CAUGHT WITH GUARDS?
                break;
            case AbilityToUnlock.ShopAutomaticItemRestocking:
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
