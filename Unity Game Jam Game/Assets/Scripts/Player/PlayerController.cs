using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    public InputActionAsset InputActions;

    private InputAction horizontal;
    private InputAction jump;
    private InputAction timeButton;
    private InputAction interact;

    [Header("Player Stats")]
    public float playerSpeed;
    public float jumpStrength;
    public float groundCheckDistance;
    public float extraFallSpeed;
    public float maxFallSpeed;
    public float timeFreezeDuration;
    public float parryRange;
    public float parryCooldownTime;
    public float timeFreezeAmount;

    public bool canParry = true;
    private bool cannotDie;

    private float moveValue;
    private bool isFacingRight = true;

    [Header("Hide stats")]
    public bool isHiding;
    private bool canHide;
    private bool canInteract;

    [Header("Buffer & Cyote Time")]
    private bool initiateJump;

    public float cyoteTime;
    private float cyoteTimeCounter;

    public float jumpBufferTime;
    private float jumpBufferCounter;

    [Header("Refrences")]
    private GameObject[] enemies;
    public GameObject[] children;

    public GameObject particles;
    public GameObject backGround;
    public GameObject hideObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private Animator anim;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        boxCollider = transform.GetComponent<BoxCollider2D>();

        horizontal = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        timeButton = InputSystem.actions.FindAction("TimeButton");
        interact = InputSystem.actions.FindAction("Interact");
    }

    private void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }


    private void Update()
    {
        float interactValue = interact.ReadValue<float>();
        if (!isHiding)
        {
            moveValue = horizontal.ReadValue<Vector2>().x;

            if (moveValue > 0.1f || moveValue < -0.1f)
            {
                anim.SetBool("Running", true);
            }
            else
            {
                anim.SetBool("Running", false);
            }

            ParryCheck();
            Flip();
            CyoteTimeAndJumpBuffering();
            IncreaseFallSpeed();

            if (interactValue == 1 && canHide && canInteract)
            {
                Hide();
            }
        }
        else
        {
            if (interactValue == 1 && canInteract)
            {
                UnHide();
            }
        }
        
        if (interactValue == 0)
        {
            canInteract = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveValue * playerSpeed, rb.linearVelocity.y);

        if (cyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            initiateJump = false;
            Jump();
        }
    }

    private void Hide()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(false);
        }

        canInteract = false;
        isHiding = true;
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        hideObject.GetComponent<HidObject>().Hide();
    }

    private void UnHide()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
        }

        canInteract = false;
        isHiding = false;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        hideObject.GetComponent<HidObject>().UnHide();
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpStrength, ForceMode2D.Impulse);
    }

    private void ParryCheck()
    {
        float timeValue = timeButton.ReadValue<float>();
        if (timeValue == 1 && canParry)
        {
            GameObject enemy = FindClosestEnemy();
            float enemyCheckDistance = Mathf.Abs(this.transform.position.x - enemy.transform.position.x);

            anim.SetTrigger("Parry");

            if (enemy.GetComponent<EnemyPatrol>().canBeParried && enemyCheckDistance <= parryRange && canParry)
            {
                StartCoroutine(Parried());
            }

            StartCoroutine(ParryCooldown());
        }
    }

    IEnumerator Parried()
    {
        cannotDie = true;

        backGround.GetComponent<LerpBackground>().LerpOpacity(timeFreezeDuration);
        var ps = particles.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpeed = 0.1f;

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Animator>().speed = timeFreezeAmount;
            enemy.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
            Physics2D.IgnoreCollision(this.boxCollider, enemy.GetComponent<BoxCollider2D>(), true);
        }

        yield return new WaitForSeconds(timeFreezeDuration);

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Animator>().speed = 1;
            Physics2D.IgnoreCollision(this.boxCollider, enemy.GetComponent<BoxCollider2D>(), false);
        }

        main.simulationSpeed = 1f;

        cannotDie = false;
    }

    IEnumerator ParryCooldown()
    {
        canParry = false;
        yield return new WaitForSeconds(parryCooldownTime);
        canParry = true;
    }

    private void Flip()
    {
        if (isFacingRight && moveValue < 0f || !isFacingRight && moveValue > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void CyoteTimeAndJumpBuffering()
    {
        if (isGrounded())
        {
            anim.SetBool("Jumping", false);
            cyoteTimeCounter = cyoteTime;
        }
        else
        {
            anim.SetBool("Jumping", true);
            cyoteTimeCounter -= Time.deltaTime;
        }

        float jumpValue = jump.ReadValue<float>();
        if (jumpValue == 1)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (cyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            initiateJump = true;
        }
        else
        {
            initiateJump = false;
        }
    }

    private void IncreaseFallSpeed()
    {
        if (rb.linearVelocity.y < 0f && rb.linearVelocity.y > maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * extraFallSpeed);
        }
        else if (rb.linearVelocity.y <= maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HideableObject")
        {
            hideObject = collision.gameObject;
            canHide = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HideableObject")
        {
            canHide = false;
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in enemies)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public void Die()
    {
        if (!cannotDie)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneBuildIndex = currentScene.buildIndex;
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
