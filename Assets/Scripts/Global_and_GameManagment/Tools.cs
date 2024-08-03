using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
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

    /* General */
    public void PauseGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = false;
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = true;
        Time.timeScale = 1f;
    }

    public GameObject FindClosestObjectByTag(Vector2 scanOrigin, float scanSize = 15f, string tag = "Enemy")
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(scanOrigin, scanSize, (Vector2)transform.position);
        foreach (RaycastHit2D enemy in nearbyEnemies)
        {
            if (enemy.transform.CompareTag(tag))
            {
                float distance = Vector3.Distance(enemy.point, scanOrigin);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy.transform.gameObject;
                }
            }
        }

        return closestTarget;
    }

    public IEnumerator DestroyAfterTime(GameObject objectToDestroy, float timeUntilDestroyed)
    {
        yield return new WaitForSeconds(timeUntilDestroyed);
        Destroy(objectToDestroy);
    }

    public Dictionary<string, float> FindDirectionOfMouseFromPlayer(Vector3 mousePositionWorld, Transform midReference)
    {
        mousePositionWorld.z = midReference.transform.position.z;

        Vector3 mousePositionPlayer = mousePositionWorld - midReference.transform.position;
        mousePositionPlayer.Normalize();

        //Dictionary<string, float> resultDict = new() { { "AttackH", mousePositionPlayer.x }, { "AttackV", mousePositionPlayer.y } };

        return new Dictionary<string, float> { { "AttackH", mousePositionPlayer.x }, { "AttackV", mousePositionPlayer.y } };
    }

    public string GetDirectionAsString(float AttackH, float AttackV)
    {
        string direction = "Unknown";

        if (AttackV > 0.7f)
        {
            direction = "Up";
        }
        else if (AttackV < -0.7f)
        {
            direction = "Down";
        }
        else if (AttackH > 0.7f)
        {
            direction = "Right";
        }
        else if (AttackH < -0.7f)
        {
            direction = "Left";
        }

        return direction;
    }
}
