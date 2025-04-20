using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Image icon;
    public Image fill;
    public Gradient gradient;

    private void Awake()
    {
        SetStamina(1);
    }

    public void SetStamina(float stamina)
    {
        slider.value = stamina;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        icon.color = gradient.Evaluate(slider.normalizedValue);
    }
}
