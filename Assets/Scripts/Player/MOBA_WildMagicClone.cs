using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MOBA_WildMagicClone : MonoBehaviour
{
    Coroutine activeAI;

    void Start()
    {
        Destroy(gameObject, 30f);
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (gameObject.name.Equals(GameAssets.i.WASDCharacter.name) || gameObject.name.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
        {
            activeAI = StartCoroutine(WASDCharacterAI());
        }
        else
        {
            activeAI = StartCoroutine(MOBACharacterAI());
        }
    }

    public void OnPlayerDied()
    {
        StopCoroutine(activeAI);
    }

    IEnumerator WASDCharacterAI()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator MOBACharacterAI()
    {
        MOBACharacter MOBAScript = GetComponent<MOBACharacter>();
        MOBAScript.firedByAi = true;
        yield return new WaitForSeconds(3.5f);

        while (true)
        {
            MOBAScript.OnAbilityOne();
            yield return new WaitForSeconds(3.5f);

            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(MOBAScript.abilityTwoCooldownRapid + 0.5f);
            MOBAScript.OnAbilityTwo();
            yield return new WaitForSeconds(3.5f);

            MOBAScript.OnAbilityThree();
            yield return new WaitForSeconds(3.5f);
        }
    }
}
