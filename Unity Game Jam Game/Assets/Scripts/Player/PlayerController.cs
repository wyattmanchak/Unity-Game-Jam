using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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

    private float moveValue;
    private bool isFacingRight = true;

    [Header("Hide stats")]
    private bool isHiding;
    private bool canHide;

    [Header("Buffer & Cyote Time")]
    private bool initiateJump;

    public float cyoteTime;
    private float cyoteTimeCounter;

    public float jumpBufferTime;
    private float jumpBufferCounter;

    [Header("Refrences")]

    private Rigidbody2D rb;

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
        rb = transform.GetComponent<Rigidbody2D>();

        horizontal = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        timeButton = InputSystem.actions.FindAction("TimeButton");
        interact = InputSystem.actions.FindAction("Interact");
    }


    private void Update()
    {
        moveValue = horizontal.ReadValue<Vector2>().x;

        Flip();
        CyoteTimeAndJumpBuffering();
        IncreaseFallSpeed();

        float interactValue = interact.ReadValue<float>();
        if (interactValue == 1 && canHide)
        {
            Hide();
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
        isHiding = true;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpStrength, ForceMode2D.Impulse);
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
            cyoteTimeCounter = cyoteTime;
        }
        else
        {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "HideableObject")
        {
            canHide = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "HideableObject")
        {
            canHide = false;
        }
    }
}
