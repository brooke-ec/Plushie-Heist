using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    //Missing setting the icon
    public Image icon;
    [SerializeField] private Image imageCooldown;
    [SerializeField] private TextMeshProUGUI keyText;

    // Start is called before the first frame update
    void Start()
    {
        imageCooldown.fillAmount = 0;
    }

    public void UpdateUI(float cooldown, float maxCooldownVal)
    {
        imageCooldown.fillAmount = 1-(cooldown / maxCooldownVal);
    }
}
