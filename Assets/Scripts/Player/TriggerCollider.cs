using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    public WASDCharacter WASDCharacterScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            WASDCharacterScript.dashCooldownReduction += 0.5f;  // Dash cooldown is reduced by 0.5 seconds for each enemy past through.
        }
    }
}
