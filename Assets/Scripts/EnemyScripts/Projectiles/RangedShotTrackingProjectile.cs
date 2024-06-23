using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedShotTrackingProjectile : MonoBehaviour
{
    [SerializeField] private AudioClip[] dealDamageClips;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private int damage = 25;

    private Transform target;
    private Vector2 movement;
    private Rigidbody2D rb;

    public Vector3 direction;
    
    public static bool tracking;
    [SerializeField] private float trackingTime;
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        tracking = true;

        StartCoroutine(DestroyAfterTime(2f));
        StartCoroutine(ProjectileTrackingTimer(trackingTime));
    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }


    void Update()
    {
        if (tracking) 
        {
            direction = target.position - transform.position;     

            //float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) * 0.01f;

            Vector2 lookDir = direction;
            transform.up = lookDir;

            direction.Normalize();
            movement = direction;
        }
    }

    IEnumerator ProjectileTrackingTimer(float time)
    {
        yield return new WaitForSeconds(time);
        tracking = false;
    }

    void FixedUpdate()
    {
        MoveProjectile(movement);
    }

    private void MoveProjectile(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (projectileSpeed * Time.deltaTime * dir));
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);

            SoundManager.instance.RandomizeSfx(dealDamageClips);
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            SoundManager.instance.RandomizeSfx(dealDamageClips);
        }
    }
}
