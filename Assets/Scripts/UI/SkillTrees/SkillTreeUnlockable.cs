using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill tree unlockable list", menuName = "Scriptable Objects/Skill tree/New skill tree unlockable list")]
public class SkillTreeUnlockable : ScriptableObject
{
    public int skillTreeNumber = 0;
    public List<Skill> skillsToEnable = new List<Skill>();
}
