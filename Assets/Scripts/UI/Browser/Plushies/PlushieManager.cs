using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushieManager : MonoBehaviour
{
    public List<PlushieInfo> plushies = new List<PlushieInfo>();

    [SerializeField] private PlushieInfoUI plushieInfoUIPrefab;

    [SerializeField] private Transform leftPlushieContainer;
    [SerializeField] private Transform rightPlushieContainer;
    private List<PlushieInfoUI> plushieInfoUIs = new List<PlushieInfoUI>();
    //NEED TO LOAD SKILLS FIRST, THEN PLUSHIE MANAGER

    List<SkillTreeController> skillTreeControllers = new List<SkillTreeController>();

    private void Start()
    {
        skillTreeControllers = FindAnyObjectByType<SkillTreesManager>().GetSkillTreeControllers();
        CreateAllPlushieContainers();
    }

    private void CreateAllPlushieContainers()
    {
        for (int i = 0; i < 6; i++)
        {
            plushies[i].plushieNumber = i;
            if(i<3)
            {
                CreatePlushieInfoUI(leftPlushieContainer, plushies[i]);
            }
            else
            {
                CreatePlushieInfoUI(rightPlushieContainer, plushies[i]);
            }
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
}
