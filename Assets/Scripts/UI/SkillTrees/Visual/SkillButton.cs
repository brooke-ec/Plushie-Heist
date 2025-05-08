using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [HideInInspector] public Skill skill;
    public Image background;
    public Image icon;
    private SkillTreeController skillTreeController;

    public bool branchIsEnabled = true;
    /// <summary>
    /// by default this is a skill tree button so you interact, but in the case of the plushies I want this false
    /// </summary>
    [HideInInspector] public bool interactableSkill = true;
    private string plushieName = "plushie";

    private void Start()
    {
        GetComponent<TooltipFunctionality>().GetTooltipInfo += GetTooltipInfo;
    }
    private void GetTooltipInfo(TooltipFunctionality tooltip)
    {
        HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges;
        Color32 textColour;

        GetPlushieInfo();
        string lockedString = "Locked until "+plushieName;
        if ((CanBeUnlocked() && IsBranchVisible()) | IsUnlocked())
        {
            tooltipBackgroundColor = skillTreeController.skillTree.palette.tooltipBackgroundColor;
            textColour = skillTreeController.skillTree.palette.upgradedTextColour;
        }
        else
        {
            tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.grey;
            textColour = skillTreeController.skillTree.palette.notUpgradedTextColour;
        }

        if (IsBranchVisible())
        {
            lockedString = "Unlocked by " + plushieName;
        }


        tooltip.SetInfo(skill.skillName, textColour, skill.description, HoveringManager.TooltipCost.coins, skill.cost.ToString(), lockedString, tooltipBackgroundColor);
    }

    public void SetUI(SkillTreeController skillTreeController)
    {
        this.skillTreeController = skillTreeController;
        icon.sprite = skill.icon;
        UpdateUI();
    }

    public void SetUI(Skill skill, SkillTreeController skillTreeController)
    {
        this.skill = skill;
        SetUI(skillTreeController);
    }

    private void UpdateUI()
    {
        UpdateColours();
        if (interactableSkill)
        {
            UpdateParentEdges();
        }
    }

    public void TryGetSkill()
    {
        if (IsBranchVisible() && CanBeUnlocked())
        {
            //Then unlock
            ShopManager.instance.ModifyMoney(-skill.cost);
            skillTreeController.unlockedSkills.Add(skill);
            UpdateUI();

            //Then update UI of any children
            Transform skillsContainer = skillTreeController.canvasTransform;

            foreach (SkillButton childNode in skillTreeController.canvasTransform.GetComponentsInChildren<SkillButton>())
            {
                //a child
                if (childNode.skill.requirements.Contains(skill))
                {
                    childNode.UpdateUI();
                }
            }
            AudioManager.instance.PlaySound(AudioManager.SoundEnum.coins);
            skill.Unlock();
        }
        else
        {
            AudioManager.instance.PlaySound(AudioManager.SoundEnum.error);
            Debug.Log("Cannot get skill " + skill.skillName);
        }
    }

    private bool IsUnlocked()
    {
        return skillTreeController.IsSkillUnlocked(skill);
    }

    private bool CanBeUnlocked()
    {
        if (IsUnlocked()) { return false; }

        //TO-DO properly
        if (ShopManager.instance.GetMoney() < skill.cost)
        {
            return false;
        }

        foreach (Skill requirement in skill.requirements)
        {
            if (!skillTreeController.IsSkillUnlocked(requirement))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsBranchVisible()
    {
        //TO-DO
        return branchIsEnabled;
    }
    private void UpdateColours()
    {
        if (skillTreeController.IsSkillUnlocked(skill))
        {
            background.sprite = skillTreeController.skillTree.palette.unlockedSprite;
            background.color = skillTreeController.skillTree.palette.unlockedColour;
            icon.color = skillTreeController.skillTree.palette.unlockedIconColour;

        }
        else
        {
            if (!IsBranchVisible())
            {
                background.sprite = skillTreeController.skillTree.palette.lockedSprite;
                background.color = skillTreeController.skillTree.palette.greyedOut;
                icon.color = skillTreeController.skillTree.palette.greyedOut;
            }
            else if (CanBeUnlocked())
            {
                background.sprite = skillTreeController.skillTree.palette.canBeUpgradedSprite;
                background.color = Color.white;
                icon.color = skillTreeController.skillTree.palette.canBeUpgradedIconColour;
            }
            else
            {
                background.sprite = skillTreeController.skillTree.palette.lockedSprite;
                background.color = Color.white;
                icon.color = skillTreeController.skillTree.palette.lockedIconColour;
            }
        }
    }

    private void UpdateParentEdges()
    {
        Color32 colour = Color.white;
        if (skillTreeController.IsSkillUnlocked(skill))
        {
            colour = skillTreeController.skillTree.palette.unlockedLineColour;
        }
        else if (!IsBranchVisible())
        {
            colour = skillTreeController.skillTree.palette.greyedOut;
        }

        Transform edgesContainer = skillTreeController.edgeContainer;
        List<EdgeRenderer> edgesFromParents = new List<EdgeRenderer>();

        //Change edge colour
        foreach (Skill requirement in skill.requirements)
        {
            Transform edge = edgesContainer.Find("Edge " + requirement.name + " to " + skill.name);
            if (edge != null)
            {
                edge.GetComponent<EdgeRenderer>().ChangeColourOfEdgeRenderer(colour);
            }
            else
            {
                Debug.LogWarning("No edge found from " + requirement.name + " to " + skill.name);
            }
        }
    }

    private void GetPlushieInfo()
    {
        PlushieManager plushieManager = FindAnyObjectByType<PlushieManager>(FindObjectsInactive.Include);
        if(plushieManager==null) { Debug.LogError("Plushie manager is null when trying to get plushie info for a skill"); return; }

        if (plushieName == "plushie") {
            //NEED TO FIX NULL REF ERROR HERE
            PlushieInfo info = plushieManager.GetPlushieForSkill(skill);

            //if null it means it's one of the default skills with no plushie associated
            if(info==null)
            {
                plushieName = "default";
            }
            else
            {
                plushieName = info.name;
            }
        }

    }

}
