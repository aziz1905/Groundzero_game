using UnityEngine;

// TANGGUNG JAWAB:
// 1. Attach di prefab Coin
// 2. Detect collision dengan Player
// 3. Play SFX & destroy coin (pure gimmick)
public class CoinCollector : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip coinSFX; // Drag audio clip di sini
    
    [Header("Optional - Particle Effect")]
    [SerializeField] private GameObject collectEffect; // Particle saat diambil (optional)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah yang nabrak adalah Player
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        // 1. Play SFX
        if (coinSFX != null)
        {
            // Pakai AudioSource.PlayClipAtPoint biar audio tetep kedengeran
            // meskipun gameObject-nya udah di-destroy
            AudioSource.PlayClipAtPoint(coinSFX, transform.position, 1f);
            
            Debug.Log("SFX playing: " + coinSFX.name); // Debug check
        }
        else
        {
            Debug.LogWarning("Coin SFX belum di-assign!");
        }

        // 2. Spawn particle effect (optional)
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 3. Destroy coin
        Destroy(gameObject);
    }
}