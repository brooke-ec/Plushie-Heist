using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlushieInfoUI : MonoBehaviour
{
    [SerializeField] private Transform topShadow;
    [SerializeField] private Transform bottomShadow;
    [SerializeField] private Transform topContainer;
    [SerializeField] private Transform bottomContainer;
    [SerializeField] private Transform skillsContainer;

    [SerializeField] private SkillButton skillPrefab;
    [SerializeField] private Sprite upArrowIcon;
    [SerializeField] private Sprite downArrowIcon;
    [SerializeField] private Sprite unlockedIcon;
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private Sprite topTrim;
    [SerializeField] private Sprite topTrimSquareBottom;

    [HideInInspector] public PlushieInfo plushieInfo;
    private PlushieManager plushieManagerRef;
    public void SetUpUI(PlushieInfo plushieInfo, PlushieManager plushieManagerRef)
    {
        this.plushieInfo = plushieInfo;
        this.plushieManagerRef = plushieManagerRef;

        //set up top
        SetUpPlushieTop();
        topContainer.GetChild(3).GetComponent<Button>().onClick.AddListener(() => OpenOrCloseInfo());

        //Now setup plushie bottom
        foreach (SkillTreeUnlockable unlockables in plushieInfo.unlockableSkills)
        {
            //for each skill tree
            foreach (Skill skill in unlockables.skillsToEnable)
            {
                SkillButton skillUI = Instantiate(skillPrefab, skillsContainer);
                skillUI.interactableSkill = false;

                //so it doesn't try to buy anything
                Destroy(skillUI.GetComponent<Button>());
                Destroy(skillUI.GetComponent<LayoutElement>());

                skillUI.SetUI(skill, plushieManagerRef.GetSkillTreeController(unlockables.skillTreeNumber));
            }
        }
    }

    private bool openInfo = false;

    private void SetUpPlushieTop()
    {
        topContainer.GetChild(0).GetComponent<Image>().sprite = plushieInfo.plushieIcon;
        topContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Plushie Number "+plushieInfo.plushieNumber + "\n"+plushieInfo.plushieName;

        if (plushieInfo.unlocked)
        {
            topContainer.GetChild(2).GetComponent<Image>().sprite = unlockedIcon;
        }
        else
        {
            topContainer.GetChild(2).GetComponent<Image>().sprite = lockedIcon;
        }
    }

    public void OpenOrCloseInfo()
    {
        openInfo = !openInfo;

        if(openInfo)
        {
            topContainer.GetChild(3).GetComponent<Image>().sprite = upArrowIcon;
            topContainer.GetComponent<Image>().sprite = topTrimSquareBottom;
        }
        else
        {
            topContainer.GetChild(3).GetComponent<Image>().sprite = downArrowIcon;
            topContainer.GetComponent<Image>().sprite = topTrim;
        }

        bottomContainer.gameObject.SetActive(openInfo);
        bottomShadow.gameObject.SetActive(openInfo);
        topShadow.gameObject.SetActive(!openInfo);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }
}
