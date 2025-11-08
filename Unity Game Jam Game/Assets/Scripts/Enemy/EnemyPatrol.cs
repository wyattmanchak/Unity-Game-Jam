using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float patrolSpeed;
    public float chaseSpeed;

    public float chaseRange;
    public float attackRange;

    private bool isAttacking;
    private bool followingPlayer;
    private bool isFacingRight = true;

    [Header("Refrences")]
    private Rigidbody2D rb;
    GameObject player;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        float playerDistance = Vector3.Distance(player.transform.position, this.transform.position);

        if (playerDistance < attackRange && !isAttacking)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (followingPlayer && !isAttacking)
        {
            ChasePlayer();
        }
        else if (!followingPlayer && !isAttacking)
        {
            Patrol();
        }
    }

    private void Attack()
    {
        isAttacking = true;
    }

    private void Patrol()
    {
        if (canMoveForward())
        {
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            Flip();
        }
    }

    private void ChasePlayer()
    {
        if (canMoveForward() && playerInFront() && playerInRange())
        {
            rb.linearVelocity = new Vector2(chaseSpeed, rb.linearVelocity.y);
        }
        else if (!playerInFront())
        {
            Flip();
        }
    }

    private bool canMoveForward()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) && !Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool playerInFront()
    {
        float playerDistanceCheck = player.transform.position.x - this.transform.position.x;
        if (isFacingRight &&  playerDistanceCheck > 0)
        {
            return true;
        }
        else if (isFacingRight && playerDistanceCheck < 0)
        {
            return false;
        }
        else if (!isFacingRight && playerDistanceCheck > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool playerInRange()
    {
        float playerDistance = Vector3.Distance(player.transform.position, this.transform.position);

        if (playerDistance < chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Flip()
    {
        patrolSpeed *= -1;
        chaseSpeed *= -1;

        Vector3 localScale = transform.localScale;
        isFacingRight = !isFacingRight;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
