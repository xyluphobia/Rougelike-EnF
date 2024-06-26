using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    private PlayerController playerController;
    private PotionsAndAbilities potionsAndAbilities;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        potionsAndAbilities = playerController.GetComponent<PotionsAndAbilities>();

        gameObject.SetActive(false);
    }

    private void resumeAfterSelect() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        StopWatch.stopwatchActive = true;
    }

    public void callSpeed() {
        potionsAndAbilities.speedPotion.useSpeedPotion();

        resumeAfterSelect();
    }

    public void callHealth() {
        potionsAndAbilities.healthPotion.useHealthPotion(1.0f);

        resumeAfterSelect();
    }
}
