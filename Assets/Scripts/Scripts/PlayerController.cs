using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === Jump Settings ===
    [Header("Jump Settings")]
    [Tooltip("Kekuatan lompat saat bar di 0%")]
    public float jumpForce = 5f; // Ini sekarang jadi lompatan minimum
    [Tooltip("Kekuatan lompat saat bar di 100%")]
    public float maxJumpForce = 10f; // Ini lompatan maksimum
    
    public Transform groundCheck;
    public float groundRadius = 0.2f;

    [Tooltip("Layer untuk tanah utama")]
    public LayerMask groundLayer;
    [Tooltip("Layer untuk platform melayang")]
    public LayerMask floatingGroundLayer;

    // === Movement (HANYA DI DARAT) ===
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

    [SerializeField] private GameObject JumpBar;

    // === UI ===
    [Header("UI")]
    public ChargeIndikator chargeIndicator;

    // === Variabel Kontrol Internal ===
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isChargingJump;
    private float moveInput;
    private bool isWallBouncing = false;
    private float wallBounceTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (chargeIndicator != null)
            chargeIndicator.StopCharge();
    }

    private void Update()
    {
        // --- Cek Status ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer | floatingGroundLayer);

        // --- Perbaikan Sliding di Tanah ---
        if (isGrounded && isWallBouncing)
        {
            isWallBouncing = false;
        }

        // --- Batalkan charge jika jatuh ---
        if (isChargingJump && !isGrounded)
        {
            isChargingJump = false;
            if (chargeIndicator != null)
                chargeIndicator.StopCharge();
        }

        // --- Logika Charge Jump ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isChargingJump = true;
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (chargeIndicator != null)
                chargeIndicator.StartCharge();

            JumpBar.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            Jump();
            isChargingJump = false;

            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            JumpBar.SetActive(false);
            
        }

        // --- Logika Wall Bounce ---
        HandleWallBounce();
    }

    private void FixedUpdate()
    {
        if (isGrounded && !isChargingJump && !isWallBouncing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        float chargeValue = 0f;
        if (chargeIndicator != null)
        {
            chargeValue = chargeIndicator.GetCurrentChargeValue();
        }

        float force = Mathf.Lerp(jumpForce, maxJumpForce, chargeValue);
        rb.velocity = new Vector2(moveInput * moveSpeed, force);
    }

    // --- (Fungsi WallBounce dan Gizmos lainnya tetap sama) ---
    private bool IsOnLeftWall() { return Physics2D.OverlapCircle(leftWallCheck.position, wallCheckDistance, wallLayer); }
    private bool IsOnRightWall() { return Physics2D.OverlapCircle(rightWallCheck.position, wallCheckDistance, wallLayer); }
    
    private void HandleWallBounce()
    {
        if (isWallBouncing)
        {
            wallBounceTimer -= Time.deltaTime;
            if (wallBounceTimer <= 0) { isWallBouncing = false; }
        }
        else
        {
            if (!isGrounded)
            {
                if (IsOnRightWall() && rb.velocity.x > 0.1f) { StartWallBounce(); }
                else if (IsOnLeftWall() && rb.velocity.x < -0.1f) { StartWallBounce(); }
            }
        }
    }
    
    // --- INI FUNGSI YANG DIPERBAIKI (Teks salah sudah dihapus) ---
    private void StartWallBounce()
    {
        isWallBouncing = true;
        wallBounceTimer = wallBounceDuration;
        rb.velocity = new Vector2(-rb.velocity.x * wallBounceStrength, rb.velocity.y * wallBounceDamping);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, groundRadius); }
        if (leftWallCheck != null) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(leftWallCheck.position, wallCheckDistance); }
        if (rightWallCheck != null) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(rightWallCheck.position, wallCheckDistance); }
    }
    
    
}