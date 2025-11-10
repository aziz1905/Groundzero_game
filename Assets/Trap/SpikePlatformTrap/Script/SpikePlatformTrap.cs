using System.Collections;
using UnityEngine;

public class SpikePlatformTrap : MonoBehaviour
{
    [SerializeField] private GameObject spikes; // Objek duri
    [SerializeField] private float delay = 0.5f; // Jeda sebelum duri muncul

    private bool hasBeenTriggered = false;

    private void Start()
    {
        // Pastikan duri mati di awal
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
    }

    // Kita pakai OnCollision, bukan OnTrigger, karena player MENDARAT di atasnya
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            // Cek apakah player mendarat di atas platform (bukan dari samping)
            if (collision.contacts[0].normal.y < -0.5) // Kontak dari atas
            {
                StartCoroutine(ShowSpikes());
            }
        }
    }

    private IEnumerator ShowSpikes()
    {
        hasBeenTriggered = true;
        
        // Kasih jeda
        yield return new WaitForSeconds(delay);

        // Munculkan duri
        if (spikes != null)
        {
            spikes.SetActive(true);
            // Anda bisa tambahkan suara "swoosh" di sini
        }
    }
}