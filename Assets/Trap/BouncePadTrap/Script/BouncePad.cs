using UnityEngine;

// Menambahkan RequireComponent agar AudioSource otomatis ada
[RequireComponent(typeof(AudioSource))] // <-- BARIS BARU
public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 30f;

    // Variabel untuk menampung file suara
    [Header("Audio")] // <-- BARIS BARU
    [SerializeField] private AudioClip bounceSound; // <-- BARIS BARU

    // Variabel untuk referensi komponen AudioSource
    private AudioSource myAudioSource; // <-- BARIS BARU

    // Method Start() untuk mengambil komponen
    private void Start() // <-- BLOK BARU
    {
        myAudioSource = GetComponent<AudioSource>();
        
        // Pastikan suara tidak diputar otomatis saat game mulai
        myAudioSource.playOnAwake = false; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Cek apakah player mendarat di atas (bukan dari samping)
            if (collision.contacts[0].normal.y < -0.5) 
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Reset kecepatan vertikal player (agar pantulan konsisten)
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                    
                    // Tambahkan dorongan ke atas
                    playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                    // 🎵 Mainkan suara "boing" di sini
                    if (bounceSound != null) // <-- BLOK BARU
                    {
                        myAudioSource.PlayOneShot(bounceSound);
                    }
                }
            }
        }
    }
}