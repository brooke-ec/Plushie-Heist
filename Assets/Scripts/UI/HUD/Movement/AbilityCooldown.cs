using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    public Image icon;
    [SerializeField] private Image imageCooldown;
    [SerializeField] private TextMeshProUGUI keyText;

    internal Ability ability;

    // Start is called before the first frame update
    void Awake()
    {
        imageCooldown.fillAmount = 0;
    }

    public void UpdateUI(float cooldown, float maxCooldownVal)
    {
        if (ability.Equals(Ability.Dash) || ability.Equals(Ability.Boost))
        {
            if (cooldown == 0)
            {
                imageCooldown.fillAmount = 0;
                return;
            }
        }
        imageCooldown.fillAmount = 1 - (cooldown / maxCooldownVal);
    }
}
