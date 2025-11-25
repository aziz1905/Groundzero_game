using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
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

    // === Audio ===
    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip wallHitSound;
    public AudioClip dieSound;

    [Header("Effects")]
    public ParticleSystem jumpDust;
    public ParticleSystem landDust;
    

    // === Internal Control ===
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isChargingJump;
    private float jumpDirection = 0f;
    private bool isWallBouncing = false;
    private float wallBounceTimer;
    private bool isOnIce = false;
    private int facingDirection = 1;
    private bool wasGrounded;

    private Animator animator;
    private AudioSource myAudioSource;
    private PlayerDeathHandler DeathHandler;

    private void Start()
    {
        respawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (chargeIndicator != null)
            chargeIndicator.StopCharge();

        animator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();

        DeathHandler = gameObject.GetComponent<PlayerDeathHandler>();
        if (DeathHandler == null)
        {
            DeathHandler = gameObject.AddComponent<PlayerDeathHandler>();
        }
    }

    private void Update()
    {
        bool wasGrounded = isGrounded;
        
        isGrounded = CheckIfGrounded();

        if (isGrounded && !wasGrounded)
        {
            if (rb.velocity.y < -0.1f) 
            {
                myAudioSource.PlayOneShot(landSound);
            }
        }
        
        bool isLanding = !wasGrounded && isGrounded;

        if (isLanding)
        {
            if (landDust != null)
                landDust.Play();
        }
        
        // PENTING: Update status untuk frame berikutnya
        wasGrounded = isGrounded;

        if (isGrounded && isWallBouncing)
            isWallBouncing = false;

        if (isChargingJump && !isGrounded)
        {
            isChargingJump = false;
            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            if (animator != null)
                animator.SetBool("isCharging", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isChargingJump = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            jumpDirection = facingDirection;

            if (chargeIndicator != null)
                chargeIndicator.StartCharge();

            if (animator != null)
                animator.SetBool("isCharging", true);
        }

        if (isChargingJump)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
                jumpDirection = Mathf.Sign(horizontalInput);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            if (animator != null)
            {
                animator.SetBool("isCharging", false);
                animator.SetInteger("Direction", (int)jumpDirection);
                animator.SetBool("isJumping", true);
            }

            PerformJump();
            isChargingJump = false;

            if (chargeIndicator != null)
                chargeIndicator.StopCharge();
        }

        if (!isChargingJump && isGrounded && !isWallBouncing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
                facingDirection = (int)Mathf.Sign(horizontalInput);
        }

        HandleWallBounce();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isGrounded && !isChargingJump && !isWallBouncing)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (isOnIce)
            {
                float targetSpeed = horizontalInput * moveSpeed;
                float smoothness = 0.05f;
                float newVelocityX = Mathf.Lerp(rb.velocity.x, targetSpeed, smoothness);
                rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

                if (Mathf.Abs(horizontalInput) < 0.1f)
                    rb.velocity = new Vector2(rb.velocity.x * 0.99f, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            }
        }
    }

    // ✅ FUNGSI BARU: Cek ground dengan raycast dari bawah
    private bool CheckIfGrounded()
    {
        // Raycast dari tengah player ke bawah (lebih akurat)
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position, 
            Vector2.down, 
            groundRadius, 
            walkableLayers
        );

        // Debug line untuk lihat raycast (hapus nanti kalau sudah oke)
        Debug.DrawRay(groundCheck.position, Vector2.down * groundRadius, hit ? Color.green : Color.red);

        return hit.collider != null;
    }

    private void PerformJump()
    {
        float chargeValue = 1f;

        if (chargeIndicator != null)
            chargeValue = chargeIndicator.GetCurrentChargeValue();

        float force = Mathf.Lerp(minJumpForce, maxJumpForce, chargeValue);
        float angleInRadians = jumpAngle * Mathf.Deg2Rad;
        float xDir = jumpDirection * Mathf.Cos(angleInRadians);
        float yDir = Mathf.Sin(angleInRadians);
        Vector2 jumpVector = new Vector2(xDir, yDir).normalized;

        if (jumpDust != null) jumpDust.Play();

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * force, ForceMode2D.Impulse);
        myAudioSource.PlayOneShot(jumpSound);
    }

    // ✅ Wall bounce tetap pakai leftWallCheck & rightWallCheck (dari samping)
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
        myAudioSource.PlayOneShot(wallHitSound);
    }

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
        myAudioSource.PlayOneShot(dieSound);

        if (isChargingJump)
        {
            isChargingJump = false;
            if (chargeIndicator != null)
                chargeIndicator.StopCharge();

            if (animator != null)
                animator.SetBool("isCharging", false);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (DeathHandler != null)
        {
            DeathHandler.TriggerDeathAnimation();
        }
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = respawnPosition;
        rb.velocity = Vector2.zero;

        if (Checkpoint.activeCheckpoint != null)
        {
            GameObject respawnCamera = Checkpoint.activeCheckpoint.GetCheckpointCamera();
            if (respawnCamera != null)
            {
                SwitchingCameraZone.ActivateStageCamera(respawnCamera);
            }
        }

        if(DeathHandler != null)
        {
            DeathHandler.ResetPhysics();
        }

        if (animator != null)
        {
            animator.Play("Idle"); 
            animator.SetBool("isJumping", false);
            animator.SetBool("isCharging", false);
            animator.SetFloat("Speed", 0);
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

            if (Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }
}