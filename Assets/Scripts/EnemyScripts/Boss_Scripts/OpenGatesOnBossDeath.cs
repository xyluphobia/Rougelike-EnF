using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGatesOnBossDeath : MonoBehaviour
{
    [SerializeField] private GameObject[] gatesToOpen;

    private void OnDeath()
    {
        foreach (GameObject gate in gatesToOpen)
        {
            gate.GetComponent<Animator>().SetBool("closeGate", false);
            gate.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
