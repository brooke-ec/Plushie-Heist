using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementUIManager : MonoBehaviour
{
    public static MovementUIManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform canvasTransform;

    [SerializeField] private Transform abilitiesTransform;
    private Dictionary<Ability, AbilityCooldown> abilities = new Dictionary<Ability, AbilityCooldown>();

    [Header("Prefab References")]
    [SerializeField] private GameObject abilityCooldownTimerPrefab;

    [Header("Internal references")]
    [SerializeField] private GameObject crosshairTransform;
    [SerializeField] private StaminaBar staminaBar;

    [Header("Icons")]
    [SerializeField] private Sprite boostIcon;
    [SerializeField] private Sprite grappleIcon;
    [SerializeField] private Sprite dashIcon;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Movement UI instance already exists");
        }
        else
        {
            instance = this;
        }

        crosshairTransform.SetActive(false);
        SetupAllAbilities();
        ChangeMovementUI(Ability.None);
    }

    private void SetupAllAbilities()
    {
        AbilityCooldown grapple = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown dash = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown boost = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();

        abilities.Add(Ability.Grapple, grapple);
        grapple.ability = Ability.Grapple;
        grapple.icon.sprite = grappleIcon;

        abilities.Add(Ability.Dash, dash);
        dash.ability = Ability.Dash;
        dash.icon.sprite = dashIcon;

        abilities.Add(Ability.Boost, boost);
        boost.ability = Ability.Boost;
        boost.icon.sprite = boostIcon;
    }

    public void UpdateStaminaBar(float stamina, float maxStamina)
    {
        staminaBar.SetStamina(stamina / maxStamina);
    }

    internal void ChangeMovementUI(Ability currentAbility)
    {
        switch (currentAbility)
        {
            case Ability.None:
                crosshairTransform.SetActive(false);
                break;
            case Ability.Dash:
                crosshairTransform.SetActive(false);
                break;
            case Ability.Boost:
                crosshairTransform.SetActive(false);
                break;
            case Ability.Grapple:
                crosshairTransform.SetActive(true);
                break;
            case Ability.Glide:
                crosshairTransform.SetActive(false);
                break;
            default:
                break;
        }

        foreach(KeyValuePair<Ability, AbilityCooldown> ability in abilities)
        {
            if(ability.Key.Equals(currentAbility))
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

        if (abilities.ContainsKey(ability))
        {
            abilities[ability].UpdateUI(cooldown, maxCooldownVal);
        }
    }
}
