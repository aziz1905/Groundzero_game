using System.Collections;
using UnityEngine;

public class TriggerSpikeTrap : MonoBehaviour
{
    // Objek duri yang akan diaktifkan
    [SerializeField] private GameObject spikes; 
    
    // Jeda waktu dari saat player menyentuh trigger sampai duri muncul
    [SerializeField] private float activationDelay = 0.5f; 

    private bool hasBeenTriggered = false;

    private void Start()
    {
        // Pastikan duri nonaktif di awal
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek player & pastikan trigger ini hanya aktif sekali
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            
            // Nonaktifkan trigger ini agar tidak berjalan berkali-kali
            GetComponent<Collider2D>().enabled = false;

            // Mulai Coroutine untuk memunculkan duri setelah jeda
            StartCoroutine(ShowSpikesAfterDelay());
        }
    }

    private IEnumerator ShowSpikesAfterDelay()
    {
        // Tunggu sesuai waktu delay
        yield return new WaitForSeconds(activationDelay);

        // Aktifkan objek duri
        if (spikes != null)
        {
            spikes.SetActive(true);
            // Tambahkan suara "swoosh" di sini jika mau
        }

        // (Opsional) Hancurkan objek trigger ini karena sudah tidak berguna
        Destroy(gameObject);
    }
}