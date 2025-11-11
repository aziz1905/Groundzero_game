using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Variabel static untuk melacak checkpoint mana yang sedang aktif
    public static Checkpoint activeCheckpoint;

    // Sprite untuk status aktif dan nonaktif
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Mulai dalam keadaan nonaktif
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah itu player DAN checkpoint ini belum aktif
        if (other.CompareTag("Player") && !isActivated)
        {
            // 1. Dapatkan skrip player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 2. Beri tahu player posisi respawn baru
                player.SetNewRespawnPoint(transform.position);

                // 3. Matikan checkpoint LAMA (jika ada)
                if (activeCheckpoint != null)
                {
                    activeCheckpoint.Deactivate();
                }

                // 4. Aktifkan checkpoint INI
                Activate();
            }
        }
    }

    // Fungsi untuk mengaktifkan checkpoint ini
    private void Activate()
    {
        isActivated = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = activeSprite;
        }

        // Set checkpoint ini sebagai yang aktif secara global
        activeCheckpoint = this;
        
        // (Opsional) Putar suara atau partikel
        // audioSource.Play();
    }

    // Fungsi PUBLIC agar bisa dipanggil oleh checkpoint lain
    public void Deactivate()
    {
        isActivated = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }
}