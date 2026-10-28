using UnityEngine;
using UnityEngine.InputSystem;

// Attach to your player sprite. Needs a Rigidbody2D and a Collider2D.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float shortHopMultiplier = 0.5f; // releasing early = smaller jump

    [Header("Ground check")]
    [SerializeField] Transform groundCheck;          // empty child placed at the player's feet
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    Animator animator; // optional
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (JumpPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Variable jump height: let go early and the jump is cut short
        if (JumpReleased() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * shortHopMultiplier);
        }

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
    }

    bool JumpPressed()
    {
        var kb = Keyboard.current;
        var pointer = Pointer.current; // mouse click or screen tap
        return (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
            || (pointer != null && pointer.press.wasPressedThisFrame);
    }

    bool JumpReleased()
    {
        var kb = Keyboard.current;
        var pointer = Pointer.current;
        return (kb != null && (kb.spaceKey.wasReleasedThisFrame || kb.upArrowKey.wasReleasedThisFrame))
            || (pointer != null && pointer.press.wasReleasedThisFrame);
    }

    // Obstacles use trigger colliders, so we detect hits here
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
