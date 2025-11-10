using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === Jump Settings ===
    [Header("Jump Settings")]
    [Tooltip("Kekuatan lompat minimum (charge 0%)")]
    public float minJumpForce = 5f;
    [Tooltip("Kekuatan lompat maksimum (charge 100%)")]
    public float maxJumpForce = 15f;
    [Tooltip("Sudut lompatan diagonal (derajat dari horizontal)")]
    public float jumpAngle = 75f;
    
    public Transform groundCheck;
    public float groundRadius = 0.2f;

    private Vector3 respawnPosition;
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
    public ChargeIndikator chargeIndicator; // Front indicator (deprecated, gunakan array dibawah)
    [Tooltip("UI Charge Indicator untuk bagian depan")]
    public ChargeIndikator chargeIndicatorFront;
    [Tooltip("UI Charge Indicator untuk bagian belakang")]
    public ChargeIndikator chargeIndicatorBack;


    // === Variabel Kontrol Internal ===
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isChargingJump;
    private float jumpDirection = 0f; 
    private bool isWallBouncing = false;
    private float wallBounceTimer;

    private void Start()
    {
        respawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        // Stop semua charge indicators saat start
        StopAllChargeIndicators();
    }
    
    // Helper method untuk start charge di semua indicators
    private void StartAllChargeIndicators()
    {
        // Backward compatibility: jika chargeIndicator di-set, gunakan itu
        if (chargeIndicator != null)
        {
            chargeIndicator.StartCharge();
        }
        
        // Gunakan Front dan Back jika di-set
        if (chargeIndicatorFront != null)
        {
            chargeIndicatorFront.StartCharge();
        }
        
        if (chargeIndicatorBack != null)
        {
            chargeIndicatorBack.StartCharge();
        }
    }
    
    // Helper method untuk stop charge di semua indicators
    private void StopAllChargeIndicators()
    {
        // Backward compatibility: jika chargeIndicator di-set, gunakan itu
        if (chargeIndicator != null)
        {
            chargeIndicator.StopCharge();
        }
        
        // Gunakan Front dan Back jika di-set
        if (chargeIndicatorFront != null)
        {
            chargeIndicatorFront.StopCharge();
        }
        
        if (chargeIndicatorBack != null)
        {
            chargeIndicatorBack.StopCharge();
        }
    }
    
    // Helper method untuk mendapatkan charge value (prioritas: Front > Back > chargeIndicator lama)
    private float GetChargeValue()
    {
        if (chargeIndicatorFront != null)
        {
            return chargeIndicatorFront.GetCurrentChargeValue();
        }
        else if (chargeIndicatorBack != null)
        {
            return chargeIndicatorBack.GetCurrentChargeValue();
        }
        else if (chargeIndicator != null)
        {
            return chargeIndicator.GetCurrentChargeValue();
        }
        return 0f;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer | floatingGroundLayer);

        if (isGrounded && isWallBouncing)
        {
            isWallBouncing = false;
        }

        if (isChargingJump && !isGrounded)
        {
            isChargingJump = false;
            StopAllChargeIndicators();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                isChargingJump = true;
                jumpDirection = Mathf.Sign(horizontalInput); 
                
                rb.velocity = new Vector2(0, rb.velocity.y);

                StartAllChargeIndicators();

                if (JumpBar != null)
                    JumpBar.SetActive(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            PerformJump();
            isChargingJump = false;

            StopAllChargeIndicators();

            if (JumpBar != null)
                JumpBar.SetActive(false);
        }

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

    private void PerformJump()
    {
        float chargeValue = GetChargeValue();

        float force = Mathf.Lerp(minJumpForce, maxJumpForce, chargeValue);
        
        force = Mathf.Max(force, minJumpForce * 0.5f);

        float angleInRadians = jumpAngle * Mathf.Deg2Rad;
        float xDir = jumpDirection * Mathf.Cos(angleInRadians);
        float yDir = Mathf.Sin(angleInRadians);
        Vector2 jumpVector = new Vector2(xDir, yDir).normalized;

        rb.velocity = Vector2.zero;
        
        // Apply jump force
        rb.AddForce(jumpVector * force, ForceMode2D.Impulse);
    }

    // --- Wall Bounce Logic ---
    private bool IsOnLeftWall() 
    { 
        return Physics2D.OverlapCircle(leftWallCheck.position, wallCheckDistance, wallLayer); 
    }
    
    private bool IsOnRightWall() 
    { 
        return Physics2D.OverlapCircle(rightWallCheck.position, wallCheckDistance, wallLayer); 
    }
    
    private void HandleWallBounce()
    {
        if (isWallBouncing)
        {
            wallBounceTimer -= Time.deltaTime;
            if (wallBounceTimer <= 0) 
            { 
                isWallBouncing = false; 
            }
        }
        else
        {
            if (!isGrounded)
            {
                if (IsOnRightWall() && rb.velocity.x > 0.1f) 
                { 
                    StartWallBounce(); 
                }
                else if (IsOnLeftWall() && rb.velocity.x < -0.1f) 
                { 
                    StartWallBounce(); 
                }
            }
        }
    }
    
    private void StartWallBounce()
    {
        isWallBouncing = true;
        wallBounceTimer = wallBounceDuration;
        rb.velocity = new Vector2(-rb.velocity.x * wallBounceStrength, rb.velocity.y * wallBounceDamping);
    }

    private void OnDrawGizmosSelected()
    {
        // Ground check
        if (groundCheck != null) 
        { 
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius); 
        }
        
        // Wall checks
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
        
        // Jump direction preview (saat charging)
        if (isChargingJump && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            float angleInRadians = jumpAngle * Mathf.Deg2Rad;
            
            // jumpDirection pasti ada value (-1 atau 1)
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

        if (isChargingJump)
        {
            isChargingJump = false;
            StopAllChargeIndicators();
            if (JumpBar != null)
                JumpBar.SetActive(false);
        }
    }
}