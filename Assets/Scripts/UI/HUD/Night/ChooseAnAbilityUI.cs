using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAnAbilityUI : MonoBehaviour
{
    [SerializeField] private GameObject abilityContainerPrefab;
    [SerializeField] private Button button;
    [SerializeField] private Transform containerParentTransform;

    [SerializeField] private Sprite blueTrimSprite;
    [SerializeField] private Sprite whiteBackgroundSprite;

    private Ability currentlySelectedAbility;

    [SerializeField] private List<(Ability, string, GameObject)> abilities = new List<(Ability, string, GameObject)>();

    private void Start()
    {
        //Setting up
        //add an ability thing for each one
        List<(Ability, string, Sprite)> allAbilitiesInfo = MovementUIManager.instance.GetAllAbilitiesInfo();

        foreach((Ability, string, Sprite) info in allAbilitiesInfo)
        {
            GameObject abilityContainer = Instantiate(abilityContainerPrefab, containerParentTransform);
            abilityContainer.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = info.Item3;
            abilityContainer.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = info.Item2;
            abilityContainer.GetComponent<Button>().onClick.AddListener(() => SelectAbility(info.Item1));

            abilities.Add((info.Item1, info.Item2, abilityContainer));
        }

        SelectAbility(abilities[0].Item1);

        button.onClick.AddListener(() => CloseChooseAbilityUI());
    }

    internal void SelectAbility(Ability ability)
    {
        foreach((Ability, string, GameObject) abilityInfo in abilities)
        {
            //if it's that ability, do blue border
            if(ability.Equals(abilityInfo.Item1))
            {
                abilityInfo.Item3.transform.GetChild(1).GetComponent<Image>().sprite = blueTrimSprite;
                currentlySelectedAbility = ability;
            }
            else
            {
                abilityInfo.Item3.transform.GetChild(1).GetComponent<Image>().sprite = whiteBackgroundSprite;
            }
        }
    }

    /// <summary>
    /// Starts the night and activates that ability chosen
    /// </summary>
    private void CloseChooseAbilityUI()
    {
        MovementUIManager.instance.ChangeMovementUI(currentlySelectedAbility);

        NightManager.instance.StartNight();
        Destroy(this.gameObject);
    }

}
