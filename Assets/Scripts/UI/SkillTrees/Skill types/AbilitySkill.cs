using UnityEngine;

[CreateAssetMenu(fileName = "New ability skill", menuName = "Scriptable Objects/Skill tree/Skills/New ability skill")]
public class AbilitySkill : Skill
{
    //Something like unlocking an ability, like wall running or grapple

    public AbilityToUnlock abilityToUnlock;
    public enum AbilityToUnlock
    {
        none,
        PlayerWallRunning,
        PlayerGrapple,
        CustomerSuggestion,
        CustomerConvincing,
        CustomerHaggling,
    }

    public override void Unlock()
    {
        switch (abilityToUnlock)
        {
            case AbilityToUnlock.PlayerWallRunning:
                UnlockWallRunning();
                break;
            default:
                break;
        }
    }

    #region Ability upgrade methods
    #region Player
    public void UnlockWallRunning()
    {
        //Something like FindAnyObjectByType<PlayerController>().UnlockWallRunning();
    }

    public void UnlockGrapple()
    {

    }
    #endregion

    #region Shop
    #endregion

    #endregion
}
