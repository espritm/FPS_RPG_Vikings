using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Image healthStats, staminaStats;

    public void DisplayHealthStats(float healthValue)
    {
        healthValue /= 100f;

        healthStats.fillAmount = healthValue;
    }

    public void DisplayStaminaStats(float staminaValue)
    {
        staminaValue /= 100f;

        staminaStats.fillAmount = staminaValue;
    }
}
