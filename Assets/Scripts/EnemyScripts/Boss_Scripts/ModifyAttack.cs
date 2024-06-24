using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyAttack : MonoBehaviour
{
    Coroutine attackPatternRoutine;
    
    // pick which attack to use based on the monster this is attached too, cast it through a standard 'modified attack' function which can be called through ranged shot
    public void TriggerModifiedAttack()
    {
        switch (gameObject.name)
        {
            case "Rotator":
                RotatorPattern script = GetComponent<RotatorPattern>();
                attackPatternRoutine = StartCoroutine(script.RotatorAttackPattern());
                break;
        }
    }

    private void OnDeath()
    {
        StopCoroutine(attackPatternRoutine);
    }
}
