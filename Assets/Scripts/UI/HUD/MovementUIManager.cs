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
    [SerializeField] private GameObject staminaBarPrefab;
    [SerializeField] private GameObject abilityCooldownTimerPrefab;

    [Header("Internal references")]
    [SerializeField] private GameObject crosshairTransform;

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
    }

    private void Start()
    {
        crosshairTransform.SetActive(false);
        SetupAllAbilities();
    }

    private void SetupAllAbilities()
    {
        AbilityCooldown grapple = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown dash = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();
        AbilityCooldown boost = Instantiate(abilityCooldownTimerPrefab, abilitiesTransform).GetComponent<AbilityCooldown>();

        abilities.Add(Ability.Grapple, grapple);
        grapple.icon.sprite = grappleIcon;
        abilities.Add(Ability.Dash, dash);
        dash.icon.sprite = dashIcon;
        abilities.Add(Ability.Boost, boost);
        boost.icon.sprite = boostIcon;
    }

    /// <summary> Activate (if passed true) or deactivate (if passed false) the crosshair</summary>
    /// <param name="activate"></param>
    public void SetCrosshairActivation(bool activate)
    {
        crosshairTransform.SetActive(activate);
    }

    internal void UpdateAbilityCooldown(Ability ability, float cooldown, float maxCooldownVal)
    {
        if (abilities.ContainsKey(ability))
        {
            abilities[ability].UpdateUI(cooldown, maxCooldownVal);
        }
    }
}
