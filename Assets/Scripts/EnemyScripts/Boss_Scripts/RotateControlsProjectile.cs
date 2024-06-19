using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateControlsProjectile : MonoBehaviour
{

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //inputHandler.moveAction.ChangeCompositeBinding("2DVector").Erase();
            //var temp = inputHandler.moveAction.GetBindingDisplayString(Up);

            /*inputHandler.moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");*/
        }
    }
}
