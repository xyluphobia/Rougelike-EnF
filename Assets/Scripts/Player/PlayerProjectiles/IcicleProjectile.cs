using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class IcicleProjectile : MonoBehaviour
{
    private Vector2 direction;
    [HideInInspector] public float projectileSpeed = 5f;
    private MOBACharacter player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<MOBACharacter>();
        StartCoroutine(Tools.instance.DestroyAfterTime(gameObject, 5f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponents<Collider2D>().Length > 1)
            {
                if (collision.gameObject.GetComponents<Collider2D>()[0].isActiveAndEnabled)
                {
                    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    GetComponent<Collider2D>().enabled = false;
                    DestroyAfterAnim();
                    return;
                }
            }

            if (player.enemyFreezeCounters.ContainsKey(collision.gameObject))
            {
                player.enemyFreezeCounters[collision.gameObject] += 1;
            }
            else
            {
                player.enemyFreezeCounters.Add(collision.gameObject, 1);
            }

            if (collision.gameObject.GetComponent<EnemyAI>() != null)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                EnemyAI enemyAi = collision.gameObject.GetComponent<EnemyAI>();
                SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                Animator animator = collision.gameObject.GetComponent<Animator>();

                switch (player.enemyFreezeCounters[collision.gameObject])
                {
                    case 1:
                        enemyAi.speed = enemyAi.defaultSpeed * 0.7f;
                        animator.speed = 0.7f;
                        spriteRenderer.color = new Color32(148, 233, 255, 255);
                        enemy.TakeDamage(5);
                        break;

                    case 2:
                        enemyAi.speed = enemyAi.defaultSpeed * 0.4f;
                        animator.speed = 0.4f;
                        spriteRenderer.color = new Color32(89, 226, 238, 255);
                        enemy.TakeDamage(15);
                        break;

                    case 3:
                        enemyAi.speed = 0f;
                        enemyAi.canMove = false;
                        animator.speed = 0f;
                        spriteRenderer.color = new Color32(0, 216, 238, 255);
                        enemy.TakeDamage(30);

                        if (player.enemyFreezeCounters[collision.gameObject] >= 3)
                            player.enemyFreezeCounters[collision.gameObject] = 0;

                        break;
                }

                enemyAi.SendMessage("OnIcicleHit");
            }

            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            DestroyAfterAnim();
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            DestroyAfterAnim();
        }
    }

    public void SetDirection(Vector3 passedInTarget)
    {
        GetComponent<Animator>().SetTrigger("Casted");

        direction = passedInTarget - transform.position;
        Vector2 newVector = direction.normalized * projectileSpeed;

        GetComponent<Rigidbody2D>().velocity = newVector;
        transform.right = direction;
    }

    public void DestroyAfterAnim(string direction = null)
    {
        if (!string.IsNullOrEmpty(direction))
        {
            transform.rotation = Quaternion.identity;
            switch (direction)
            {
                case "Up":
                    transform.rotation *= Quaternion.Euler(0, 0, 90f);
                    break;

                case "Right":
                    break;

                case "Down":
                    transform.rotation *= Quaternion.Euler(0, 0, -90f);
                    break;

                case "Left":
                    transform.rotation *= Quaternion.Euler(0, 0, -180f);
                    break;
            }
        }

        GetComponent<Animator>().SetTrigger("Impact");
        Destroy(gameObject, 0.25f * 0.7f);
    }
}
