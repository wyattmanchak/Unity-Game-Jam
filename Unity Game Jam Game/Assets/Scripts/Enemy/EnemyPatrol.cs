using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float patrolSpeed;
    public float chaseSpeed;

    public float chaseRange;
    public float attackRange;
    public float killRange;

    public bool canBeParried;

    private bool isAttacking;
    private bool isFacingRight = true;

    [Header("Refrences")]
    //public AudioClip attackSound;

    private Rigidbody2D rb;
    private Animator anim;
    GameObject player;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        float playerDistance = Vector3.Distance(player.transform.position, this.transform.position);

        if (playerDistance < attackRange && !isAttacking && !player.GetComponent<PlayerController>().isHiding && canMoveForward())
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (!isAttacking)
        {
            Patrol();
        }
    }

    private void Attack()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");
    }

    private void EndAttack()
    {
        isAttacking = false;
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

    private bool canMoveForward()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 1f, groundLayer) && !Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer))
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

    public void SetCanBeParried()
    {
        //SoundManager.instance.PlaySound(attackSound, transform, .25f, true);
        canBeParried = true;
    }

    public void SetCannotBeParried()
    {
        canBeParried = false;
    }

    public void KillPlayer()
    {
        float playerDistance = Vector3.Distance(player.transform.position, this.transform.position);

        if (playerDistance <= killRange)
        {
            player.GetComponent<PlayerController>().Die();
        }
    }
}
