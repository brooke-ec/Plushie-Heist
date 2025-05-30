using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill tree", menuName = "Scriptable Objects/Skill tree/New skill tree")]
public class SkillTree : ScriptableObject
{
    public string skillTreeName;
    public List<Skill> skills;

    public SkillTreePalette palette;
    public Sprite skillTreeIcon;

    [Header("Unlockables")]
    public List<SkillTreeUnlockable> unlockables = new List<SkillTreeUnlockable>();

}
