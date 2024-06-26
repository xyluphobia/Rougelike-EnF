using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBossOnRoomEnter : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnableScriptsOn;
    [SerializeField] private GameObject[] gatesToClose;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enableScripts();
        closeGate();
    }

    private void enableScripts()
    {
        MonoBehaviour[] scripts = objectToEnableScriptsOn.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }

    private void closeGate()
    {
        foreach (GameObject gate in gatesToClose)
        {
            gate.GetComponent<Animator>().SetBool("closeGate", true);
            gate.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
