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

    [Header("Attack keys (one per element)")]
    [SerializeField] Key fireKey = Key.A;
    [SerializeField] Key waterKey = Key.W;
    [SerializeField] Key airKey = Key.S;
    [SerializeField] Key earthKey = Key.D;

    [Header("Attack")]
    [SerializeField] Transform attackPoint;          // empty child in front of the player (optional)
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 0.1f;
    [SerializeField] bool hasAttackAnimation = false; // tick if your Animator has an "Attack" trigger

    Rigidbody2D rb;
    Animator animator; // optional
    bool isGrounded;
    float nextAttackTime;

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

        Element? pressed = GetPressedElement();
        if (pressed.HasValue)
        {
            TryAttack(pressed.Value);
        }

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
    }

    // Returns which element's key was pressed this frame, or null if none
    Element? GetPressedElement()
    {
        var kb = Keyboard.current;
        if (kb == null) return null;

        if (kb[fireKey].wasPressedThisFrame) return Element.Fire;
        if (kb[waterKey].wasPressedThisFrame) return Element.Water;
        if (kb[airKey].wasPressedThisFrame) return Element.Air;
        if (kb[earthKey].wasPressedThisFrame) return Element.Earth;
        return null;
    }

    void TryAttack(Element attackElement)
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null && hasAttackAnimation) animator.SetTrigger("Attack");

        // Attack area: the attack point if set, otherwise just in front of the player
        Vector2 center = attackPoint != null
            ? (Vector2)attackPoint.position
            : (Vector2)transform.position + Vector2.right * 0.75f;

        // Find the closest living obstacle in range
        Obstacle target = null;
        float closest = float.MaxValue;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(center, attackRange))
        {
            Obstacle obstacle = hit.GetComponentInParent<Obstacle>();
            if (obstacle == null || obstacle.IsDead) continue;

            float distance = Vector2.Distance(center, obstacle.transform.position);
            if (distance < closest)
            {
                closest = distance;
                target = obstacle;
            }
        }

        // Only counts if the key matches the obstacle's element
        if (target != null && target.ElementType == attackElement)
        {
            target.TakeHit();
        }
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

    // Running into a living obstacle costs points instead of ending the game
    void OnTriggerEnter2D(Collider2D other)
    {
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle != null)
        {
            obstacle.OnPlayerTouched();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Red circle shows the attack range in the Scene view
        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + Vector3.right * 0.75f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}