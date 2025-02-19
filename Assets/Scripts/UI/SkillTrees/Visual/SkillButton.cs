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
    private SkillTreeManager skillTreeManager;

    public void SetUI(SkillTreeManager skillTreeManager)
    {
        this.skillTreeManager = skillTreeManager;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = skill.skillName;
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
            UIManager.money -= skill.cost;
            skillTreeManager.unlockedSkills.Add(skill);
            UpdateUI();

            //Then update UI of any children
            Transform skillsContainer = skillTreeManager.canvasTransform;

            foreach(SkillButton childNode in skillTreeManager.canvasTransform.GetComponentsInChildren<SkillButton>())
            {
                //a child
                if(childNode.skill.requirements.Contains(skill))
                {
                    childNode.UpdateUI();
                }
            }
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
            if(!skillTreeManager.IsSkillUnlocked(requirement))
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
        if (skillTreeManager.IsSkillUnlocked(skill))
        {
            background.sprite = skillTreeManager.palette.unlockedSprite;
            background.color = skillTreeManager.palette.unlockedColour;
        }
        else
        {
            if (!IsBranchVisible())
            {
                background.sprite = skillTreeManager.palette.lockedSprite;
                background.color = skillTreeManager.palette.greyedOut;
            }
            else if (CanBeUnlocked())
            {
                background.sprite = skillTreeManager.palette.canBeUpgradedSprite;
                background.color = Color.white;
            }
            else
            {
                background.sprite = skillTreeManager.palette.lockedSprite;
                background.color = Color.white;
            }
        }
    }

    private void UpdateParentEdges()
    {
        Color32 colour = Color.white;
        if(skillTreeManager.IsSkillUnlocked(skill))
        {
            colour = skillTreeManager.palette.unlockedLineColour;
        }
        else if(!IsBranchVisible())
        {
            colour = skillTreeManager.palette.greyedOut;
        }

        Transform edgesContainer = skillTreeManager.edgeContainer;
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
