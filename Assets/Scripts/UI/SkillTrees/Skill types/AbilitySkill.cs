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
      
            case AbilityToUnlock.PlayerDash:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Dash); }
                break;
            case AbilityToUnlock.PlayerBoost:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Boost); }
                break;
            case AbilityToUnlock.PlayerWallRunning:
                PlayerController.instance.wallRunEnabled = true;
                break;
            case AbilityToUnlock.PlayerGrapple:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Grapple); }
                break;
            case AbilityToUnlock.PlayerGlide:
                if (isNight) { MovementUIManager.instance.LearnAbility(Ability.Glide); }
                break;
            case AbilityToUnlock.PlayerSecondChance:
                PlayerController.instance.secondChance = true;
                break;
            case AbilityToUnlock.ShopAutomaticItemRestocking:
                if (!isNight) ShopManager.instance.autoRestocking = true;
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
