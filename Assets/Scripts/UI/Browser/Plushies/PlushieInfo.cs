using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plushie", menuName = "Scriptable Objects/Plushie", order = 9999)]
public class PlushieInfo : ResourcesAsset
{
    public static PlushieInfo[] plushies => _plushies == null ? _plushies = Resources.LoadAll<PlushieInfo>(PATH).OrderBy(p => p.order).ToArray() : _plushies;
    private static PlushieInfo[] _plushies;

    public const string PATH = "Plushies/";

    public new string name;
    public int order = 0;
    public Sprite icon;
    public SkillTreeUnlockable[] unlockedSkills;
    public GameObject prefab;

    [HideInInspector] public bool unlocked => SharedUIManager.instance.plushie == null ? false : SharedUIManager.instance.plushie.order >= order;
    [HideInInspector] public PlushieInfo next => order < plushies.Length ? plushies[order] : null;
    [HideInInspector] public PlushieInfo previous => order > 0 ? plushies[order] : null;
}
