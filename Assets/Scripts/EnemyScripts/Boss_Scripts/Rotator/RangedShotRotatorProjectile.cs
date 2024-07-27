using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedShotRotatorProjectile : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerController playerControllerScript;
    private PlayerInput playerInput;

    private string currentPlayerCharacterString;
    private string originalBinds;
    private string characterName;

    void Awake()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerControllerScript = playerObject.GetComponent<PlayerController>();
        playerInput = playerObject.GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetOriginalActions(collision.name);

        if (collision.gameObject.CompareTag("Player") && !playerControllerScript.invulnerable)
        {
            if (characterName.Equals(GameAssets.i.WASDCharacter.name) || characterName.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
            {
                string stringOriginalBinds = originalBinds.ToString();
                stringOriginalBinds = stringOriginalBinds.Remove(stringOriginalBinds.Length - 1, 1).Remove(0, 14);

                string[] splitInputs = stringOriginalBinds.Split(",");
                char wasdLeft = splitInputs[0][^1];
                char wasdRight = splitInputs[1][^1];
                char wasdDown = splitInputs[2][^1];
                char wasdUp = splitInputs[3][^1];

                playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(1, $"<Keyboard>/{wasdUp}"); // wasdUp
                playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(2, $"<Keyboard>/{wasdDown}"); // wasdDown
                playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(3, $"<Keyboard>/{wasdLeft}"); // wasdLeft
                playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(4, $"<Keyboard>/{wasdRight}"); // wasdRight
            }
            else if (characterName.Equals(GameAssets.i.MOBACharacter.name) || characterName.Equals(GameAssets.i.MOBACharacter.name + "(Clone)"))
            {
                string stringOriginalBinds = originalBinds.ToString();
                string[] splitInputs = stringOriginalBinds.Split("/Keyboard/");

                char abilityOne = splitInputs[1][0];
                char abilityTwo = splitInputs[2][0];
                char abilityThree = splitInputs[3][0];
                char ultimateAbility = splitInputs[4][0];

                playerInput.currentActionMap.FindAction("AbilityOne").ApplyBindingOverride($"<Keyboard>/{ultimateAbility}"); // wasdUp
                playerInput.currentActionMap.FindAction("AbilityTwo").ApplyBindingOverride($"<Keyboard>/{abilityOne}"); // wasdDown
                playerInput.currentActionMap.FindAction("AbilityThree").ApplyBindingOverride($"<Keyboard>/{abilityTwo}"); // wasdLeft
                playerInput.currentActionMap.FindAction("UltimateAbility").ApplyBindingOverride($"<Keyboard>/{abilityThree}"); // wasdRight
            }
            else
                Debug.Log("Player type not found.");
        }
    }

    private void SetOriginalActions(string characterNamePassed)
    {
        characterName = characterNamePassed;
        if (characterNamePassed.Equals(GameAssets.i.WASDCharacter.name) || characterNamePassed.Equals(GameAssets.i.WASDCharacter.name + "(Clone)"))
        {
            originalBinds = playerInput.actions["Move"].ToString();

        }
        else if (characterNamePassed.Equals(GameAssets.i.MOBACharacter.name) || characterNamePassed.Equals(GameAssets.i.MOBACharacter.name + "(Clone)"))
        {
            originalBinds = playerInput.actions["AbilityOne"].ToString()
                + playerInput.actions["AbilityTwo"].ToString()
                + playerInput.actions["AbilityThree"].ToString()
                + playerInput.actions["UltimateAbility"].ToString();
        }
    }

    private void OnDestroy()
    {
        GameObject[] potentialRotatorObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject rotator in potentialRotatorObjects)
        {
            RotatorPattern rotatorScript = rotator.GetComponent<RotatorPattern>();
            if (rotatorScript != null)
                rotatorScript.BindHolder(originalBinds, characterName);
        }
    }
}
