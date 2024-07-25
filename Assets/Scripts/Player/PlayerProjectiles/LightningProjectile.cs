using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 direction;
    private Vector2 movement = Vector2.zero;
    private float projectileSpeed = 3.5f;

    public bool spawnedByPlayer = false;
    public List<GameObject> lightningBoltHitEnemies = new();
    public Transform target = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(Tools.instance.DestroyAfterTime(gameObject, 2f));
    }

    void Update()
    {
        if (target == null)
        {
            transform.localScale = Vector3.zero;
            return;
        }
        transform.localScale = Vector3.one;

        direction = target.position - transform.position;

        Vector2 lookDir = direction;
        transform.right = lookDir;

        direction.Normalize();
        movement = direction;
    }
    void FixedUpdate()
    {
        MoveProjectile(movement);
    }

    private void MoveProjectile(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (projectileSpeed * Time.deltaTime * dir));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10);
            // play sound effect
            // spawn any hit effects

            RaycastHit2D[] nearbyEnemies = Physics2D.CircleCastAll(transform.position, 3f, (Vector2)transform.position);
            foreach (RaycastHit2D enemy in nearbyEnemies)
            {
                if (!lightningBoltHitEnemies.Contains(enemy.transform.gameObject) && enemy.transform.CompareTag("Enemy"))  // If the enemy hasn't already been hit by this parent bolt
                {
                    GameObject bolt = Instantiate(GameAssets.i.lightningBoltProjectile, collision.transform.position, Quaternion.identity);
                    lightningBoltHitEnemies.Add(enemy.transform.gameObject);
                    bolt.GetComponent<LightningProjectile>().lightningBoltHitEnemies = lightningBoltHitEnemies;
                    bolt.GetComponent<LightningProjectile>().target = enemy.transform;
                }
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}