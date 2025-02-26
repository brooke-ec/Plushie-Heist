using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [HideInInspector] public Skill skill;
    public Image background;
    private SkillTreeController skillTreeController;

    private void Start()
    {
        GetComponent<TooltipFunctionality>().GetTooltipInfo += GetTooltipInfo;
    }
    private void GetTooltipInfo(TooltipFunctionality tooltip)
    {
        HoveringManager.TooltipBackgroundColor tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.noChanges;
        Color32 textColour;
        if (CanBeUnlocked())
        {
            tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.blue;
            textColour = skillTreeController.skillTree.palette.upgradedTextColour;
        }
        else
        {
            tooltipBackgroundColor = HoveringManager.TooltipBackgroundColor.grey;
            textColour = skillTreeController.skillTree.palette.notUpgradedTextColour;
        }
        tooltip.SetInfo(skill.skillName, textColour, skill.description, HoveringManager.TooltipCost.coins, skill.cost.ToString(), tooltipBackgroundColor);
    }

    public void SetUI(SkillTreeController skillTreeController)
    {
        this.skillTreeController = skillTreeController;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = skill.skillName;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateColours();
        UpdateParentEdges();
    }

    public void TryGetSkill()
    {
        if (IsBranchVisible() && CanBeUnlocked())
        {
            //Then unlock
            UIManager.money -= skill.cost;
            skillTreeController.unlockedSkills.Add(skill);
            UpdateUI();

            //Then update UI of any children
            Transform skillsContainer = skillTreeController.canvasTransform;

            foreach(SkillButton childNode in skillTreeController.canvasTransform.GetComponentsInChildren<SkillButton>())
            {
                //a child
                if(childNode.skill.requirements.Contains(skill))
                {
                    childNode.UpdateUI();
                }
            }
            skill.Unlock();
        }
        else
        {
            Debug.Log("Cannot get skill " + skill.skillName);
        }
    }

    private bool CanBeUnlocked()
    {
        //TO-DO properly
        if (UIManager.money < skill.cost)
        {
            return false;
        }

        foreach (Skill requirement in skill.requirements)
        {
            if(!skillTreeController.IsSkillUnlocked(requirement))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsBranchVisible()
    {
        //TO-DO
        return skill.branchIsUnlocked;
        //return true;
    }
    private void UpdateColours()
    {
        if (skillTreeController.IsSkillUnlocked(skill))
        {
            background.sprite = skillTreeController.skillTree.palette.unlockedSprite;
            background.color = skillTreeController.skillTree.palette.unlockedColour;
        }
        else
        {
            if (!IsBranchVisible())
            {
                background.sprite = skillTreeController.skillTree.palette.lockedSprite;
                background.color = skillTreeController.skillTree.palette.greyedOut;
            }
            else if (CanBeUnlocked())
            {
                background.sprite = skillTreeController.skillTree.palette.canBeUpgradedSprite;
                background.color = Color.white;
            }
            else
            {
                background.sprite = skillTreeController.skillTree.palette.lockedSprite;
                background.color = Color.white;
            }
        }
    }

    private void UpdateParentEdges()
    {
        Color32 colour = Color.white;
        if(skillTreeController.IsSkillUnlocked(skill))
        {
            colour = skillTreeController.skillTree.palette.unlockedLineColour;
        }
        else if(!IsBranchVisible())
        {
            colour = skillTreeController.skillTree.palette.greyedOut;
        }

        Transform edgesContainer = skillTreeController.edgeContainer;
        List<EdgeRenderer> edgesFromParents = new List<EdgeRenderer>();

        //Change edge colour
        foreach (Skill requirement in skill.requirements)
        {
            Transform edge = edgesContainer.Find("Edge " + requirement.skillName + " to " + skill.skillName);
            if(edge != null)
            {
                edge.GetComponent<EdgeRenderer>().ChangeColourOfEdgeRenderer(colour);
            }
            else
            {
                Debug.LogWarning("No edge found from " + requirement.skillName + " to " + skill.skillName);
            }
        }
    }

}
