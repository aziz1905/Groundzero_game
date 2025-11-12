using System.Collections;
using System.Collections.Generic; // <-- Penting untuk List
using UnityEngine;

// Skrip ini sekarang mewarisi dari Induk (BaseResettableTrap)
public class HorizontalRockTrap : ResetTraps 
{
    [Header("Pengaturan Unik Trap Ini")]
    [Tooltip("Prefab batu yang akan di-spawn")]
    [SerializeField] private GameObject rockPrefab; 
    
    [Tooltip("Titik di mana batu akan di-spawn")]
    [SerializeField] private Transform spawnPoint; 

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float destroyDelay = 4f; // Waktu sebelum batu hancur

    // List untuk melacak semua batu yang sudah di-spawn oleh trap ini
    private List<GameObject> spawnedRocks = new List<GameObject>();

    // Kita tidak perlu Start() lagi, Induk sudah mengurusnya.
    
    // --- Ini adalah logika unik untuk trap ini ---
    // (Apa yang terjadi saat trap AKTIF?)
    protected override IEnumerator ActivateTrapLogic()
    {
        // 1. Tentukan posisi spawn
        // (Jika 'spawnPoint' tidak di-set, pakai posisi trigger)
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : transform.position;

        // 2. Buat (Instantiate) batu baru dari prefab
        GameObject newRock = Instantiate(rockPrefab, pos, Quaternion.identity);
        
        // 3. Tambahkan batu baru ke list (agar bisa di-reset)
        spawnedRocks.Add(newRock); 

        // 4. Ambil Rigidbody-nya dan luncurkan
        Rigidbody2D rockRb = newRock.GetComponent<Rigidbody2D>();
        if (rockRb != null)
        {
            rockRb.isKinematic = false;
            rockRb.velocity = new Vector2(moveSpeed, 0); // Logika unik Anda!
        }
        else
        {
            Debug.LogError("Rock Prefab tidak punya Rigidbody2D!", rockPrefab);
        }

        // 5. Hancurkan batu itu setelah 'destroyDelay'
        // (Kita juga akan hapus dari list saat hancur agar rapi)
        StartCoroutine(DestroyAndRemoveRock(newRock, destroyDelay));

        yield return null; 
    }

    // --- Ini adalah logika unik untuk trap ini ---
    // (Apa yang terjadi saat di-RESET?)
    protected override void ResetTrapLogic()
    {
        // Hancurkan semua batu yang MASIH ADA
        foreach (GameObject rock in spawnedRocks)
        {
            if (rock != null) // Cek jika belum hancur
            {
                Destroy(rock);
            }
        }
        
        // Kosongkan list
        spawnedRocks.Clear();
    }

    // --- Fungsi Helper ---
    private IEnumerator DestroyAndRemoveRock(GameObject rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Hapus dari list (jika masih ada)
        if (spawnedRocks.Contains(rock))
        {
            spawnedRocks.Remove(rock);
        }
        
        // Hancurkan batu
        if (rock != null)
        {
            Destroy(rock);
        }
    }
}