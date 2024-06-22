using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.XR.TrackedPoseDriver;

public class RangedShotStandardProjectile : MonoBehaviour
{
    [SerializeField] private AudioClip[] dealDamageClips;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private int damage = 25;

    private Transform target;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Collider2D selfCollider;

    public Vector3 direction;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();

        StartCoroutine(DestroyAfterTime(2f));
    }

    void FixedUpdate()
    {
        MoveProjectile(movement);
    }

    private void MoveProjectile(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (dir * projectileSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
