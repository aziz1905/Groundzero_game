using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Vector3 respawnPosition;
    private float horizontal;
    private float speed = 5f;
    [SerializeField] private float jumpingPower = 16f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // --- VARIABEL BARU UNTUK WALL BOUNCE ---
    [Header("Wall Bounce Parameters")]
    [SerializeField] private Transform wallCheck; // Titik untuk cek dinding
    [SerializeField] private LayerMask wallLayer;   // Layer untuk dinding
    [SerializeField] private float wallCheckDistance = 0.3f; // Jarak OverlapCircle untuk dinding
    [SerializeField] private float wallBounceStrength = 0.8f; // Kekuatan pantulan (0.8 = 80% dari kecepatan)
    [SerializeField] private float wallBounceDamping = 0.7f;  // Seberapa banyak kecepatan vertikal berkurang
    [SerializeField] private float wallBounceDuration = 0.2f; // Durasi pemain tidak bisa kontrol

    private bool isWallBouncing = false;
    private float wallBounceTimer;
    // ------------------------------------------

    private void Start()
    {
        // Simpan posisi player saat ini sebagai titik respawn
        respawnPosition = transform.position;
    }
    private void Update()
    {
        // Hanya ambil input jika tidak sedang memantul
        if (!isWallBouncing)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }

        // 1. Logika Lompat
        if (Input.GetKeyDown(KeyCode.Space) && IsGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        // 2. Logika Variable Jump (Memotong lompatan) - Saya perbaiki dari GetKeyDown -> GetKeyUp
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // 3. Logika Wall Bounce
        HandleWallBounce();
    }

    private void FixedUpdate()
    {
        // Hanya terapkan gerakan horizontal jika TIDAK sedang memantul
        if (!isWallBouncing)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // --- FUNGSI BARU UNTUK CEK DINDING ---
    private bool IsOnWall()
    {
        // Cek apakah wallCheck bersentuhan dengan wallLayer
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckDistance, wallLayer);
    }

    // --- FUNGSI BARU UNTUK MENGATUR WALL BOUNCE ---
    private void HandleWallBounce()
    {
        if (isWallBouncing)
        {
            // Jika sedang memantul, kurangi timer
            wallBounceTimer -= Time.deltaTime;
            if (wallBounceTimer <= 0)
            {
                isWallBouncing = false; // Waktu pantulan habis, pemain bisa kontrol lagi
            }
        }
        else
        {
            // Cek kondisi untuk memantul:
            // 1. Menyentuh dinding (IsOnWall())
            // 2. Tidak di tanah (!IsGround())
            // 3. Bergerak ke arah dinding (rb.velocity.x > 0.1f) - spesifik untuk dinding kanan
            if (IsOnWall() && !IsGround() && rb.velocity.x > 0.1f)
            {
                // -- MULAI WALL BOUNCE --
                isWallBouncing = true;
                wallBounceTimer = wallBounceDuration;

                // Terapkan fisika pantulan
                rb.velocity = new Vector2(
                    -rb.velocity.x * wallBounceStrength, // Balikkan arah X dan kalikan dengan kekuatan
                    rb.velocity.y * wallBounceDamping   // Kurangi kecepatan Y dengan damping
                );

                // Hentikan input horizontal agar tidak menimpa pantulan
                horizontal = 0;

                // Di sini tempat Anda bisa memicu animasi atau suara
                // Contoh: animator.SetTrigger("WallBounce");
                // Contoh: audioSource.PlayOneShot(bounceSound);
            }
        }
    }

    // --- TAMBAHKAN INI UNTUK MELIHAT GIZMO DI EDITOR ---
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckDistance);
        }
    }

    // ... (di dalam kelas Movement) ...

    // FUNGSI INI AKAN DIPANGGIL OLEH TRAP
    public void DieAndRespawn()
    {
        // Kembalikan ke posisi awal
        transform.position = respawnPosition;

        // Reset kecepatan
        rb.velocity = Vector2.zero;

        // Anda bisa tambahkan efek suara/visual kematian di sini
        // Debug.Log("Player terkena trap!");
    }

    // HAPUS FUNGSI LAMA DI BAWAH INI DARI MOVEMENT.CS
    /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            // ... kode ini sudah pindah ke DieAndRespawn() ...
        }
    }
    */
}