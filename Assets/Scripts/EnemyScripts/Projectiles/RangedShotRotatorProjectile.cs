using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedShotRotatorProjectile : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerController playerControllerScript;
    private PlayerInput playerInput;

    private InputAction originalBinds;
    private string newBinds;

    void Awake()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerControllerScript = playerObject.GetComponent<PlayerController>();
        playerInput = playerObject.GetComponent<PlayerInput>();

        SetOriginalActions();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerControllerScript.invulnerable)
        {

            string stringOriginalBinds = originalBinds.ToString();
            stringOriginalBinds = stringOriginalBinds.Remove(stringOriginalBinds.Length - 1, 1).Remove(0, 14);

            string[] splitInputs = stringOriginalBinds.Split(",");
            string wasdLeft = splitInputs[0].Remove(0, 10);
            string wasdRight = splitInputs[1].Remove(0, 10);
            string wasdDown = splitInputs[2].Remove(0, 10);
            string wasdUp = splitInputs[3].Remove(0, 10);

            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(1, $"<Keyboard>/{wasdUp}"); // wasdUp
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(2, $"<Keyboard>/{wasdDown}"); // wasdDown
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(3, $"<Keyboard>/{wasdLeft}"); // wasdLeft
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(4, $"<Keyboard>/{wasdRight}"); // wasdRight
        }
    }

    private void SetOriginalActions()
    {
        originalBinds = playerInput.actions["Move"];
    }
}
