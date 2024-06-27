using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Gradient healthBarGradient;
    private float _target;

    public void setHealth (float health, float maxHealth)
    {
        float target = health / maxHealth;
        StartCoroutine(healthBarChangeOverTime(target));
        CheckHealthBarGradient(target);
    }

    private IEnumerator healthBarChangeOverTime(float healthGoal)
    {
        if (healthBar.fillAmount < healthGoal)
        {
            while (healthBar.fillAmount < healthGoal)
            {
                healthBar.fillAmount += 1 / 2f * Time.deltaTime;  // Change float between '/' and '*' to edit speed of animation.
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (healthBar.fillAmount > healthGoal)
            {
                healthBar.fillAmount -= 1 / 2f * Time.deltaTime;  // Change float between '/' and '*' to edit speed of animation.
                yield return new WaitForEndOfFrame();
            }
        }
        

        if (healthGoal <= 0)
            healthBar.fillAmount = 0;
    }

    private void CheckHealthBarGradient(float target)
    {
        healthBar.color = healthBarGradient.Evaluate(target);
    }
}
