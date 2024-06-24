using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedShotRotatorProjectile : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerController playerControllerScript;
    private PlayerInput playerInput;

    private string originalBinds;

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
            char wasdLeft = splitInputs[0][^1];
            char wasdRight = splitInputs[1][^1];
            char wasdDown = splitInputs[2][^1];
            char wasdUp = splitInputs[3][^1];

            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(1, $"<Keyboard>/{wasdUp}"); // wasdUp
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(2, $"<Keyboard>/{wasdDown}"); // wasdDown
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(3, $"<Keyboard>/{wasdLeft}"); // wasdLeft
            playerInput.currentActionMap.FindAction("Move").ApplyBindingOverride(4, $"<Keyboard>/{wasdRight}"); // wasdRight
        }
    }

    private void SetOriginalActions()
    {
        originalBinds = playerInput.actions["Move"].ToString();
    }

    private void OnDestroy()
    {
        GameObject[] potentialRotatorObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject rotator in potentialRotatorObjects)
        {
            RotatorPattern rotatorScript = rotator.GetComponent<RotatorPattern>();
            if (rotatorScript != null)
                rotatorScript.BindHolder(originalBinds);
        }
    }
}
