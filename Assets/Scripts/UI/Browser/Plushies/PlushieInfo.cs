using UnityEngine;

[CreateAssetMenu(fileName = "New Plushie", menuName = "Scriptable Objects/Plushie", order = 9999)]
public class PlushieInfo : ResourcesAsset
{
    public const string PATH = "Plushies/";

    public new string name;
    public int order = 0;
    public Sprite icon;
    public SkillTreeUnlockable[] unlockedSkills;

    [HideInInspector] public bool unlocked => SharedUIManager.instance.plushie == null ? false : SharedUIManager.instance.plushie.order >= order;
}
