using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === Jump Settings ===
    [Header("Jump Settings")]
    public float minJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float jumpAngle = 72f;
    public float fallGravityMultiplier = 1.5f;

    public Transform groundCheck;
    public float groundRadius = 0.2f;

    private Vector3 respawnPosition;
    [Tooltip("Layer APA SAJA yang bisa diinjak oleh player")]
    public LayerMask walkableLayers;

    // === Movement (Ground Only) ===
    [Header("Movement (Ground Only)")]
    public float moveSpeed = 5f;

    // === Wall Bounce Settings ===
    [Header("Wall Bounce Settings")]
    public float wallBounceStrength = 0.8f;
    public float wallBounceDamping = 0.7f;
    public float wallBounceDuration = 0.2f;

    public LayerMask wallLayer;
    public float wallCheckDistance = 0.2f;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    
    // === UI ===
    [Header("UI")]
    public ChargeIndikator chargeIndicator;
    [SerializeField] private GameObject jumpBar;

    // === Internal Control ===
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isChargingJump;
    private float jumpDirection = 0f;
    private bool isWallBouncing = false;
    private float wallBounceTimer;
    private bool isOnIce = false;
    private int facingDirection = 1;
    
    // === Animator ===
    private Animator animator;

    private void Start()
    {
        respawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (chargeIndicator != null)
            chargeIndicator.StopCharge();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // --- Ground check ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, walkableLayers);

        // --- Reset wall bounce saat di tanah ---
        if (isGrounded && isWallBouncing)
            isWallBouncing = false;

        // --- Batalkan charge jika jatuh ---
        if (isChargingJump && !isGrounded)
        {
            isChargingJump = false;
            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            if (animator != null)
                animator.SetBool("isCharging", false);
        }

        // --- Mulai charge dengan spasi ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isChargingJump = true;
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Default arah = arah hadap terakhir
            jumpDirection = facingDirection;

            if (chargeIndicator != null)
                chargeIndicator.StartCharge();

            if (jumpBar != null)
                jumpBar.SetActive(true);

            // 🔹 Aktifkan animasi charge
            if (animator != null)
                animator.SetBool("isCharging", true);
        }

        // --- Selama charging, boleh ubah arah ---
        if (isChargingJump)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
                jumpDirection = Mathf.Sign(horizontalInput);
        }

        // --- Lepas spasi untuk lompat ---
        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            if (animator != null)
            {
                animator.SetBool("isCharging", false); // 🔹 matikan charge
                animator.SetInteger("Direction", (int)jumpDirection);
                animator.SetBool("isJumping", true);   // 🔹 lompat
            }

            PerformJump();
            isChargingJump = false;

            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            if (jumpBar != null)
                jumpBar.SetActive(false);
        }

        // --- Update arah hadap player ---
        if (!isChargingJump && isGrounded && !isWallBouncing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
                facingDirection = (int)Mathf.Sign(horizontalInput);
        }

        // --- Handle wall bounce ---
        HandleWallBounce();

        // === Update Animator ===
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // Hanya bisa gerak di darat, tidak sedang charge, tidak wall bounce
        if (isGrounded && !isChargingJump && !isWallBouncing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (isOnIce)
            {
                // ===== Efek licin (momentum inertia) =====
                float targetSpeed = horizontalInput * moveSpeed;
                float smoothness = 0.05f; // Semakin kecil => semakin licin
                float newVelocityX = Mathf.Lerp(rb.velocity.x, targetSpeed, smoothness);

                rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

                // Sedikit terus meluncur walau input dilepas
                if (Mathf.Abs(horizontalInput) < 0.1f)
                    rb.velocity = new Vector2(rb.velocity.x * 0.99f, rb.velocity.y);
            }
            else
            {
                // ===== Gerak normal =====
                rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            }
        }
    }

    private void PerformJump()
    {
        float chargeValue = 1f;

        // Ambil nilai charge dari indikator (kalau ada)
        if (chargeIndicator != null)
            chargeValue = chargeIndicator.GetCurrentChargeValue();

        // Konversi charge 0–1 jadi kekuatan antara min dan max
        float force = Mathf.Lerp(minJumpForce, maxJumpForce, chargeValue);

        float angleInRadians = jumpAngle * Mathf.Deg2Rad;
        float xDir = jumpDirection * Mathf.Cos(angleInRadians);
        float yDir = Mathf.Sin(angleInRadians);
        Vector2 jumpVector = new Vector2(xDir, yDir).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * force, ForceMode2D.Impulse);
    }


    // === Wall Bounce ===
    private bool IsOnLeftWall() => Physics2D.OverlapCircle(leftWallCheck.position, wallCheckDistance, wallLayer);
    private bool IsOnRightWall() => Physics2D.OverlapCircle(rightWallCheck.position, wallCheckDistance, wallLayer);

    private void HandleWallBounce()
    {
        if (isWallBouncing)
        {
            wallBounceTimer -= Time.deltaTime;
            if (wallBounceTimer <= 0) isWallBouncing = false;
        }
        else
        {
            if (!isGrounded)
            {
                if (IsOnRightWall() && rb.velocity.x > 0.1f) StartWallBounce();
                else if (IsOnLeftWall() && rb.velocity.x < -0.1f) StartWallBounce();
            }
        }
    }

    private void StartWallBounce()
    {
        isWallBouncing = true;
        wallBounceTimer = wallBounceDuration;
        rb.velocity = new Vector2(-rb.velocity.x * wallBounceStrength, rb.velocity.y * wallBounceDamping);
    }

    // === Deteksi Platform Es (pakai Tag) ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "IcePlatform")
            isOnIce = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "IcePlatform")
            isOnIce = false;
    }

    // === Gizmos untuk debugging ===
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (leftWallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(leftWallCheck.position, wallCheckDistance);
        }
        if (rightWallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightWallCheck.position, wallCheckDistance);
        }

        if (isChargingJump && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            float angleInRadians = jumpAngle * Mathf.Deg2Rad;
            float xDir = jumpDirection * Mathf.Cos(angleInRadians);
            float yDir = Mathf.Sin(angleInRadians);
            Vector2 jumpVector = new Vector2(xDir, yDir).normalized;
            Gizmos.DrawRay(transform.position, jumpVector * 3f);
        }
    }

    public void DieAndRespawn()
    {
        transform.position = respawnPosition;
        rb.velocity = Vector2.zero;

        GameManager.Instance.PlayerHasDied();

        if (isChargingJump)
        {
            isChargingJump = false;
            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            if (animator != null)
                animator.SetBool("isCharging", false);
        }
    }
    
    public void SetNewRespawnPoint(Vector3 newPosition)
    {
        Debug.Log("Checkpoint baru ditetapkan di: " + newPosition);
        respawnPosition = newPosition;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", speed);

        if (isGrounded)
        {
            animator.SetInteger("Direction", facingDirection);

            // Reset jumping flag saat mendarat
            if (Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }
}
