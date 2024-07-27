using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    [HideInInspector] public float defaultSpeed;
    public float attackRadius;

    [SerializeField] private float checkRadius;
    [SerializeField] private bool shouldRotate;

    [SerializeField] private LayerMask PlayerLayer;

    private Transform target;
    private Animator animator;
    private NavMeshAgent agent;
    public bool canMove = true;
    public Vector2 movement;

    private Vector2 lastFramePos;
    private Vector2 currentFramePos;

    private bool isInChaseRange;
    public bool isInAttackRange;
    [HideInInspector] public bool chasePlayer = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponentInParent<NavMeshAgent>();
    }
    void Start()
    {
        lastFramePos = transform.position;
        StartCoroutine(EnableAiAfter(2.0f));
        this.enabled = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    IEnumerator EnableAiAfter(float time)
    {
        yield return new WaitForSeconds(time);
        this.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            agent.ResetPath();
            return;
        }

        isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, PlayerLayer);
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, PlayerLayer);

        if (shouldRotate)
        {
            UpdateAnimation();
        }

        if (isInChaseRange && !isInAttackRange)
        {
            UpdateAnimation();
            agent.SetDestination(target.position);
        }
        if (isInChaseRange && chasePlayer)
        {
            UpdateAnimation();
            agent.SetDestination(target.position);
        }

        if (isInAttackRange && !chasePlayer)
        {
            agent.ResetPath();

            Vector2 diffVector = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
            float diffXPercent = diffVector.x / (diffVector.x + diffVector.y);
            float diffYPercent = diffVector.y / (diffVector.x + diffVector.y);

            if (target.position.x < transform.position.x)
                diffXPercent *= -1;
            if (target.position.y < transform.position.y)
                diffYPercent *= -1;

            animator.SetFloat("attackH", diffXPercent);
            animator.SetFloat("attackV", diffYPercent);

            animator.SetFloat("Horizontal", diffXPercent);
            animator.SetFloat("Vertical", diffYPercent);

            if (diffXPercent > 0.7f || diffXPercent < -0.7f || diffYPercent > 0.7f || diffYPercent < -0.7f)
            {
                animator.SetFloat("lastHorizontal", diffXPercent);
                animator.SetFloat("lastVertical", diffYPercent);
            }
        }
    }

    public void OnDeath()
    {
        agent.enabled = false;
    }

    private void UpdateAnimation()
    {
        currentFramePos = transform.position;

        if (currentFramePos != lastFramePos)
        {
            animator.SetBool("IsMoving", true);

            Vector2 diffVector = new Vector2(System.Math.Abs(currentFramePos.x - lastFramePos.x), System.Math.Abs(currentFramePos.y - lastFramePos.y));

            float diffXPercent = diffVector.x / (diffVector.x + diffVector.y);
            float diffYPercent = diffVector.y / (diffVector.x + diffVector.y);

            if (currentFramePos.x < lastFramePos.x)
                diffXPercent *= -1;
            if (currentFramePos.y < lastFramePos.y)
                diffYPercent *= -1;

            movement = new Vector2(diffXPercent, diffYPercent);

            animator.SetFloat("Horizontal", diffXPercent);
            animator.SetFloat("Vertical", diffYPercent);

            lastFramePos = currentFramePos;

            if (diffXPercent > 0.7f || diffXPercent < -0.7f || diffYPercent > 0.7f || diffYPercent < -0.7f)
            {
                animator.SetFloat("lastHorizontal", diffXPercent);
                animator.SetFloat("lastVertical", diffYPercent);
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
