using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalRockTrap : ResetTraps 
{
    [Header("Pengaturan Unik Trap Ini")]
    [Tooltip("Prefab batu yang akan di-spawn")]
    [SerializeField] private GameObject rockPrefab; 
    
    [Tooltip("Titik di mana batu akan di-spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Movement Settings")]
    [Tooltip("Arah peluncuran batu")]
    [SerializeField] private RockDirection direction = RockDirection.Right;
    
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float destroyDelay = 4f;

    public enum RockDirection
    {
        Right,  
        Left,  
        Up,     
        Down,   
        Custom  
    }

    [Header("Custom Direction (Jika pilih Custom)")]
    [Tooltip("Arah custom dalam derajat (0 = kanan, 90 = atas, 180 = kiri, 270 = bawah)")]
    [SerializeField] private float customAngle = 0f;

    // List untuk melacak semua batu yang sudah di-spawn
    private List<GameObject> spawnedRocks = new List<GameObject>();

    protected override IEnumerator ActivateTrapLogic()
    {
        // 1. Tentukan posisi spawn
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : transform.position;

        // 2. Buat batu baru
        GameObject newRock = Instantiate(rockPrefab, pos, Quaternion.identity);
        
        // 3. Tambahkan ke list
        spawnedRocks.Add(newRock); 

        // 4. Hitung arah velocity berdasarkan pilihan
        Vector2 velocity = GetVelocityFromDirection();

        // 5. Ambil Rigidbody dan luncurkan
        Rigidbody2D rockRb = newRock.GetComponent<Rigidbody2D>();
        if (rockRb != null)
        {
            rockRb.isKinematic = false;
            rockRb.velocity = velocity;
        }
        else
        {
            Debug.LogError("Rock Prefab tidak punya Rigidbody2D!", rockPrefab);
        }

        // 6. Hancurkan setelah delay
        StartCoroutine(DestroyAndRemoveRock(newRock, destroyDelay));

        yield return null; 
    }

    protected override void ResetTrapLogic()
    {
        // Hancurkan semua batu yang masih ada
        foreach (GameObject rock in spawnedRocks)
        {
            if (rock != null)
            {
                Destroy(rock);
            }
        }
        
        spawnedRocks.Clear();
    }

    private Vector2 GetVelocityFromDirection()
    {
        switch (direction)
        {
            case RockDirection.Right:
                return new Vector2(moveSpeed, 0);
                
            case RockDirection.Left:
                return new Vector2(-moveSpeed, 0);
                
            case RockDirection.Up:
                return new Vector2(0, moveSpeed);
                
            case RockDirection.Down:
                return new Vector2(0, -moveSpeed);
                
            case RockDirection.Custom:
                float angleInRadians = customAngle * Mathf.Deg2Rad;
                float x = Mathf.Cos(angleInRadians) * moveSpeed;
                float y = Mathf.Sin(angleInRadians) * moveSpeed;
                return new Vector2(x, y);
                
            default:
                return new Vector2(moveSpeed, 0); 
        }
    }

    private IEnumerator DestroyAndRemoveRock(GameObject rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (spawnedRocks.Contains(rock))
        {
            spawnedRocks.Remove(rock);
        }
        
        if (rock != null)
        {
            Destroy(rock);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.red;
        
        // Gambar arrow untuk lihat arah peluncuran
        Vector2 dir = GetVelocityFromDirection().normalized;
        Vector3 start = spawnPoint.position;
        Vector3 end = start + (Vector3)dir * 2f; 
        
        Gizmos.DrawLine(start, end);
        
        // Gambar kepala panah
        Vector3 arrowHead1 = end - (Vector3)(Quaternion.Euler(0, 0, 30) * dir) * 0.5f;
        Vector3 arrowHead2 = end - (Vector3)(Quaternion.Euler(0, 0, -30) * dir) * 0.5f;
        Gizmos.DrawLine(end, arrowHead1);
        Gizmos.DrawLine(end, arrowHead2);
    }
}