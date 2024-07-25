using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Tools : MonoBehaviour
{
    public static Tools instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator DestroyAfterTime(GameObject objectToDestroy, float timeUntilDestroyed)
    {
        yield return new WaitForSeconds(timeUntilDestroyed);
        Destroy(objectToDestroy);
    }
}
