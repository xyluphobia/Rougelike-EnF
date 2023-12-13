using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float attackRadius;

    [SerializeField] private float checkRadius;
    [SerializeField] private bool shouldRotate;

    [SerializeField] private LayerMask PlayerLayer;

    private Transform target;
    private Rigidbody2D rb;
    private Animator animator;
    public Vector2 movement;

    public Vector3 direction;

    private bool isInChaseRange;
    public bool isInAttackRange;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        StartCoroutine(EnableAiAfter(2.0f));
        this.enabled = false;
    }

    IEnumerator EnableAiAfter(float time)
    {
        yield return new WaitForSeconds(time);
        this.enabled = true;
    }

    private void Update()
    {
        isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, PlayerLayer);
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, PlayerLayer);

        direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        direction.Normalize();
        movement = direction;

        if (shouldRotate)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
    }

    private void FixedUpdate()
    {
        if (isInChaseRange && !isInAttackRange)
        {
            animator.SetBool("IsMoving", isInChaseRange);
            MoveCharacter(movement);
        }

        if (isInAttackRange)
        {
            animator.SetBool("IsMoving", false);
            rb.velocity = Vector2.zero;
        }
    }


    private void MoveCharacter(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (dir * speed * Time.deltaTime));
    }
}
