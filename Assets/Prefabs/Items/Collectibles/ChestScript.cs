using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private GameObject chest;

    [Header("Spawned Items")]
    [SerializeField] private GameObject item1;
    [SerializeField] private GameObject item2;
    [SerializeField] private GameObject item3;

    [SerializeField] private GameObject loc1;
    [SerializeField] private GameObject loc2;
    [SerializeField] private GameObject loc3;

    private Animator animator;
    private bool opened = false;

    void Start()
    {
        animator = chest.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !opened)
        {
            opened = true;
            animator.SetTrigger("OpenChest");
            SoundManager.instance.PlaySound(GameAssets.i.chestOpenClip);

            StartCoroutine(SpawnCoinsOnOpen());
        }
    }

    IEnumerator SpawnCoinsOnOpen()
    {
        yield return new WaitForSeconds(0.4f);

        Instantiate(item1, loc1.transform.position, Quaternion.identity);
        Instantiate(item2, loc2.transform.position, Quaternion.identity);
        Instantiate(item3, loc3.transform.position, Quaternion.identity);
    }
}
