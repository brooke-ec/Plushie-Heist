using System.Collections.Generic;
using UnityEngine;

public class MovementUIManager : MonoBehaviour
{
    public static MovementUIManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform canvasTransform;

    [SerializeField] private Transform abilitiesTransform;
    private Dictionary<Ability, AbilityCooldown> abilities = new Dictionary<Ability, AbilityCooldown>();

    private List<Ability> currentlyLearnedAbilities = new List<Ability>();

    [Header("Prefab References")]
    [SerializeField] private GameObject abilityCooldownTimerPrefab;

    [Header("Internal references")]
    [SerializeField] private GameObject crosshairTransform;
    [SerializeField] private StaminaBar staminaBar;

    [Header("Icons")]
    [SerializeField] private Sprite boostIcon;
    [SerializeField] private Sprite grappleIcon;
    [SerializeField] private Sprite dashIcon;
    [SerializeField] private Sprite glideIcon;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Movement UI instance already exists");
        }
        else
        {
            instance = this;
        }

        SetupAllAbilities();
        ChangeMovementUI(Ability.None);
    }

    private void Update()
    {
        crosshairTransform.SetActive(PlayerController.instance.currentAbility == Ability.Grapple || PlayerController.instance._holdingBeanBag);
    }

    private void SetupAllAbilities()
    {
        AbilityCooldown grapple = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown dash = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown boost = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown glide = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();

        abilities.Add(Ability.Grapple, grapple);
        grapple.ability = Ability.Grapple;
        grapple.icon.sprite = grappleIcon;

        abilities.Add(Ability.Dash, dash);
        dash.ability = Ability.Dash;
        dash.icon.sprite = dashIcon;

        abilities.Add(Ability.Boost, boost);
        boost.ability = Ability.Boost;
        boost.icon.sprite = boostIcon;

        abilities.Add(Ability.Glide, glide);
        glide.ability = Ability.Glide;
        glide.icon.sprite = glideIcon;
    }

    internal void LearnAbility(Ability ability)
    {
        if(!currentlyLearnedAbilities.Contains(ability))
        {
            currentlyLearnedAbilities.Add(ability);
        }
    }

    public void UpdateStaminaBar(float stamina, float maxStamina)
    {
        staminaBar.SetStamina(stamina / maxStamina);
    }

    internal void ChangeMovementUI(Ability currentAbility)
    {
        foreach (KeyValuePair<Ability, AbilityCooldown> ability in abilities)
        {
            if (ability.Key.Equals(currentAbility))
            {
                ability.Value.gameObject.SetActive(true);
            }
            else
            {
                ability.Value.gameObject.SetActive(false);
            }
        }

    }

    internal void UpdateAbilityCooldown(Ability ability, float cooldown, float maxCooldownVal)
    {
        if (!currentlyLearnedAbilities.Contains(ability)) return;

        if (abilities.ContainsKey(ability))
        {
            abilities[ability].UpdateUI(cooldown, maxCooldownVal);
        }
    }

    /// <summary>
    /// Gets abilities info of ONLY LEARNT ABILITIES
    /// </summary>
    /// <returns></returns>
    internal List<(Ability, string, Sprite)> GetAllAbilitiesInfo()
    {
        List<(Ability, string, Sprite)> abilitiesInfo = new List<(Ability, string, Sprite)>();
        foreach(Ability ability in abilities.Keys)
        {
            if (currentlyLearnedAbilities.Contains(ability))
            {
                abilitiesInfo.Add(GetAbilityInfo(ability));
            }
        }
        return abilitiesInfo;
    }

    private (Ability, string, Sprite) GetAbilityInfo(Ability ability)
    {
        switch (ability)
        {
            case Ability.Dash:
                return (ability, "Dash", dashIcon);
            case Ability.Boost:
                return (ability, "Boost", boostIcon);
            case Ability.Grapple:
                return (ability, "Grapple", grappleIcon);
            case Ability.Glide:
                return (ability, "Glide", glideIcon);
            default:
                return (ability, null, null);
        }
    }
}
