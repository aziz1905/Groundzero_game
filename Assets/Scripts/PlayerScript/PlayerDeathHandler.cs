using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody2D))]
public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    public float deathJumpForce = 5f;      // Kekuatan loncat saat mati
    public float deathGravity = 3f;         // Gravitasi saat jatuh mati (biar cepat)
    public float delayBeforeRespawn = 2.0f; // Waktu tunggu animasi jatuh selesai

    private PlayerController playerController;
    private Rigidbody2D rb;
    private BoxCollider2D boxcol;
    private CircleCollider2D circol;
    private float defaultGravity;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        boxcol = GetComponent<BoxCollider2D>();
        circol = GetComponent<CircleCollider2D>(); // Ambil Collider (Box/Capsule/Circle)
        defaultGravity = rb.gravityScale;
    }

    // Fungsi ini dipanggil saat kena Trap
    public void TriggerDeathAnimation()
    {
        // 1. Matikan kontrol player
        playerController.enabled = false;

        // 2. Matikan collider agar tembus tanah/trap
        if(boxcol != null) boxcol.enabled = false;
        if(circol != null) circol.enabled = false;

        // 3. Reset velocity biar tidak terlempar ke samping
        rb.velocity = Vector2.zero;
        
        // 4. Ubah gravitasi jika perlu (biar jatuhnya dramatis)
        rb.gravityScale = deathGravity;

        // 5. Loncat ke atas! (Mario Style)
        rb.AddForce(Vector2.up * deathJumpForce, ForceMode2D.Impulse);

        // 6. Mainkan Sound (Kita ambil dari referensi PlayerController atau play di sini)
        // Opsional: pindahkan logic play sound die ke sini jika mau

        // 7. Mulai hitung mundur untuk lapor ke GameManager
        StartCoroutine(WaitAndTellGameManager());
    }

    // Fungsi untuk mengembalikan kondisi fisik player saat Respawn
    public void ResetPhysics()
    {
        Time.timeScale = 1f;

        rb.gravityScale = defaultGravity;
        rb.velocity = Vector2.zero;
        if(boxcol != null) boxcol.enabled = true;
        if(circol != null) circol.enabled = true;
        
        // Nyalakan kembali script controller
        playerController.enabled = true;
    }

    private IEnumerator WaitAndTellGameManager()
    {
        // Tunggu sampai animasi jatuh kira-kira selesai (misal 2 detik)
        yield return new WaitForSeconds(delayBeforeRespawn);

        // Panggil GameManager untuk kurangi nyawa & fade screen
        // Logic ini dipindahkan dari PlayerController ke sini
        GameManager.Instance.StartDeathSequence();
    }
}