using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlushieManager : MonoBehaviour
{
    [HideInInspector] public PlushieInfo[] plushies;

    [SerializeField] private PlushieInfoUI plushieInfoUIPrefab;

    [SerializeField] private Transform leftPlushieContainer;
    [SerializeField] private Transform rightPlushieContainer;
    private List<PlushieInfoUI> plushieInfoUIs = new List<PlushieInfoUI>();
    //NEED TO LOAD SKILLS FIRST, THEN PLUSHIE MANAGER

    List<SkillTreeController> skillTreeControllers = new List<SkillTreeController>();

    private void Awake()
    {
        plushies = Resources.LoadAll<PlushieInfo>(PlushieInfo.PATH).OrderBy(p => p.order).ToArray();
    }

    private void Start()
    {
        skillTreeControllers = FindAnyObjectByType<SkillTreesManager>().GetSkillTreeControllers();
        CreateAllPlushieContainers();
    }

    private void CreateAllPlushieContainers()
    {
        foreach (PlushieInfo plushie in  plushies)
        {
            if(plushie.order % 2 == 1) CreatePlushieInfoUI(leftPlushieContainer, plushie);
            else CreatePlushieInfoUI(rightPlushieContainer, plushie);
        }
    }

    private void CreatePlushieInfoUI(Transform container, PlushieInfo plushieInfo)
    {
        PlushieInfoUI plushieInfoUI = Instantiate(plushieInfoUIPrefab, container);
        plushieInfoUI.SetUpUI(plushieInfo, this);
        plushieInfoUIs.Add(plushieInfoUI);
    }

    public SkillTreeController GetSkillTreeController(int skillTreeNumber)
    {
        return skillTreeControllers[skillTreeNumber];
    }

    public PlushieInfo GetPlushieForSkill(Skill skill)
    {
        //basically look for the unlockable lists in the plushies list, if this skill is here then return that plushie info
        PlushieInfo plushieInfo = plushies.FirstOrDefault(p => p.unlockedSkills.FirstOrDefault(
            unlockables => unlockables.skillsToEnable.Contains(skill)) != null);
        return plushieInfo;
    }
}
