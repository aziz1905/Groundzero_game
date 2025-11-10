using System.Collections;
using UnityEngine;

public class FallingRockTrap : MonoBehaviour
{
    // 1. Masukkan objek Batu (Rock) Anda ke sini lewat Inspector
    [SerializeField] private GameObject rockObject;

    // 2. Atur berapa lama batu akan hancur setelah aktif (misal: 5 detik)
    [SerializeField] private float rockDestroyDelay = 5f;

    // 3. Variabel untuk memastikan trap hanya aktif sekali
    private bool hasBeenTriggered = false;

    // 4. Kita simpan referensi ke collider trigger ini
    private Collider2D trapCollider;

    private void Start()
    {
        // Ambil komponen collider dari objek trigger INI
        trapCollider = GetComponent<Collider2D>();

        // Pastikan Anda sudah menonaktifkan batu (rock) di awal permainan
        // (Anda bisa lakukan ini manual di Inspector)
        // if (rockObject != null)
        // {
        //     rockObject.SetActive(false); 
        // }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah yang masuk adalah Player DAN trap ini belum aktif
        if (collision.CompareTag("Player") && !hasBeenTriggered)
        {
            // === LANGKAH 1: MENCEGAH BUG ===
            // Langsung tandai & matikan collider agar tidak bisa ter-trigger lagi
            hasBeenTriggered = true;
            trapCollider.enabled = false; // Ini bekerja instan!

            // === LANGKAH 2: AKTIFKAN BATU ===
            if (rockObject != null)
            {
                // Aktifkan batu (misalnya membuatnya terlihat dan jatuh)
                rockObject.SetActive(true);

                // (Opsional) Jika batu Anda pakai Rigidbody2D untuk jatuh:
                Rigidbody2D rockRb = rockObject.GetComponent<Rigidbody2D>();
                if (rockRb != null)
                {
                    // Set 'Is Kinematic' ke false agar batu mulai jatuh kena gravitasi
                    rockRb.isKinematic = false;
                }
            }
            
            // === LANGKAH 3: JALANKAN COROUTINE ===
            // Mulai hitung mundur untuk menghancurkan batu
            StartCoroutine(DestroyRockAfterDelay());
        }
    }

    // === LANGKAH 4: COROUTINE (JEDA WAKTU) ===
    private IEnumerator DestroyRockAfterDelay()
    {
        // "Tunggu selama 'rockDestroyDelay' detik"
        yield return new WaitForSeconds(rockDestroyDelay);

        // "Setelah selesai menunggu, hancurkan objek BATU"
        if (rockObject != null)
        {
            Destroy(rockObject);
        }

        // (Opsional tapi rapi) Hancurkan juga objek TRAP TRIGGER ini
        // karena tugasnya sudah selesai.
        Destroy(gameObject); // 'gameObject' di sini adalah si objek trigger
    }
}