using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;

    public void setHealth (float health, float maxHealth)
    {
        healthBar.fillAmount = health / maxHealth;
    }
}
