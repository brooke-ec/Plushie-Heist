using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Manages ALL skill trees </summary>
public class SkillTreesManager : MonoBehaviour
{
    [SerializeField] private GameObject skillTreePrefab;

    [SerializeField] private Transform skillTreeViewTransform;
    [SerializeField] private Button skillTreeButtonSwitch;
    [SerializeField] private Transform coinsContainer;

    [SerializeField] private List<SkillTreeController> skillTrees = new List<SkillTreeController>();
    [SerializeField] private List<SkillTree> skillTreesReferences = new List<SkillTree>();


    private void Start()
    {
        CreateAllSkillTrees();
        skillTrees[currentlyActiveSkillTree].gameObject.SetActive(true);
        ChangeSkillButtonLook();
        ShopManager.onMoneyChanged.AddListener(UpdateCoins);
        ShopManager.onMoneyChanged.AddListener(UpdateSkillsUI);

        skillTreeButtonSwitch.onClick.AddListener(() => SwitchSkillTree());

        for (int i = 0; i < SharedUIManager.instance.plushieIndex; i++) EnableNextBranch();

        //EnableNextBranch();
    }

    public void CreateAllSkillTrees()
    {
        for (int i = 0; i < skillTreesReferences.Count; i++)
        {
            skillTrees.Add(Instantiate(skillTreePrefab, skillTreeViewTransform).GetComponent<SkillTreeController>());
            skillTrees[i].CreateSkillTree(skillTreesReferences[i]);
            skillTrees[i].gameObject.SetActive(false);
        }
        //skillTreeViewTransform.gameObject.SetActive(false);
    }

    private bool isSkillTreeViewOpen = false;
    public void OpenSkillTreeView()
    {
        isSkillTreeViewOpen = !isSkillTreeViewOpen;
        skillTreeViewTransform.gameObject.SetActive(isSkillTreeViewOpen);
    }

    public void UpdateSkillsUI()
    {
        foreach (SkillTreeController skillTree in skillTrees)
        {
            skillTree.UpdateUIOfSkills();
        }
    }

    #region Switching between skill trees
    public int currentlyActiveSkillTree = 0;

    public void SwitchSkillTree()
    {
        skillTreeViewTransform.GetChild(currentlyActiveSkillTree).gameObject.SetActive(false);
        //player
        if (currentlyActiveSkillTree == 0)
        {
            currentlyActiveSkillTree = 1;
        }
        else
        {
            //shop
            currentlyActiveSkillTree = 0;
        }
        ChangeSkillButtonLook();

        skillTreeViewTransform.GetChild(currentlyActiveSkillTree).gameObject.SetActive(true);
    }

    private void ChangeSkillButtonLook()
    {
        int buttonLook = 0;
        //player
        if (currentlyActiveSkillTree == 0) { buttonLook = 1; }
        else { buttonLook = 0; }

        Sprite newButtonBackground = skillTrees[buttonLook].skillTree.palette.canBeUpgradedSprite;
        Sprite newButtonIcon = skillTrees[buttonLook].skillTree.skillTreeIcon;
        Color32 newButtonIconColour = skillTrees[buttonLook].skillTree.palette.unlockedLineColour;

        skillTreeButtonSwitch.transform.GetChild(1).GetComponent<Image>().sprite = newButtonBackground;
        skillTreeButtonSwitch.transform.GetChild(2).GetComponent<Image>().sprite = newButtonIcon;
        skillTreeButtonSwitch.transform.GetChild(2).GetComponent<Image>().color = newButtonIconColour;

        UpdateCoins();
    }

    private void UpdateCoins()
    {
        Sprite newButtonBackground = skillTrees[currentlyActiveSkillTree].skillTree.palette.canBeUpgradedSprite;
        coinsContainer.GetChild(1).GetComponent<Image>().sprite = newButtonBackground;

        coinsContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = ShopManager.instance.GetMoney().ToString("n2");
    }

    #endregion

    #region User control

    private int nextPlushieToBeRescued = 0;

    /// <summary> Enables skills from rescuing plushieNumber. Also enables any parent skills so pay attention </summary>
    /// <param name="plushieNumber">Number of plushie to rescue: pay attention to order in list</param>
    public void EnableNextBranch()
    {
        foreach (var tree in skillTrees) tree.EnableBranch(nextPlushieToBeRescued);
        nextPlushieToBeRescued++;
    }

    public List<SkillTreeController> GetSkillTreeControllers()
    {
        return skillTrees;
    }
    #endregion
}
