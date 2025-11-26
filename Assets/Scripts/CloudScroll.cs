using UnityEngine;

public class CloudScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("Kecepatan scroll ke kanan (positif = kanan, negatif = kiri)")]
    [SerializeField] private float scrollSpeed = 2f;
    
    [Header("Loop Settings - Auto Calculate")]
    [Tooltip("Jarak tambahan setelah keluar layar sebelum reset")]
    [SerializeField] private float extraDistance = 2f;
    
    [Header("Optional - Random Speed")]
    [Tooltip("Tambah variasi kecepatan random (0 = tidak ada random)")]
    [SerializeField] private float speedVariation = 0f;
    
    private float actualSpeed;
    private Camera mainCamera;
    private float rightBoundary;
    private float leftBoundary;
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        // Ambil main camera
        mainCamera = Camera.main;
        
        // Ambil sprite renderer untuk ukuran sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Hitung boundary layar
        CalculateBoundaries();
        
        // Tambahkan random speed jika ada variation
        actualSpeed = scrollSpeed + Random.Range(-speedVariation, speedVariation);
    }
    
    private void CalculateBoundaries()
    {
        if (mainCamera != null)
        {
            // Hitung posisi kanan dan kiri layar dalam world space
            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            
            // Tambahkan ukuran sprite agar smooth
            float spriteWidth = 0f;
            if (spriteRenderer != null)
            {
                spriteWidth = spriteRenderer.bounds.size.x;
            }
            
            // Boundary kanan = setengah layar + lebar sprite + extra
            rightBoundary = (cameraWidth / 2f) + spriteWidth + extraDistance;
            
            // Boundary kiri = negatif dari kanan
            leftBoundary = -rightBoundary;
        }
        else
        {
            // Fallback jika tidak ada camera
            rightBoundary = 15f;
            leftBoundary = -15f;
        }
    }
    
    private void Update()
    {
        // Gerakkan awan ke kanan
        transform.Translate(Vector3.right * actualSpeed * Time.deltaTime);
        
        // Kalau sudah keluar layar kanan, reset ke kiri
        if (transform.position.x > rightBoundary)
        {
            ResetPosition();
        }
    }
    
    private void ResetPosition()
    {
        Vector3 newPos = transform.position;
        newPos.x = leftBoundary;
        transform.position = newPos;
        
        // Random speed lagi untuk variasi
        if (speedVariation > 0)
        {
            actualSpeed = scrollSpeed + Random.Range(-speedVariation, speedVariation);
        }
    }
    
    // Debug gizmo untuk lihat boundary
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && mainCamera != null)
        {
            Gizmos.color = Color.red;
            
            // Garis kanan (reset point)
            Gizmos.DrawLine(
                new Vector3(rightBoundary, -10, 0),
                new Vector3(rightBoundary, 10, 0)
            );
            
            // Garis kiri (spawn point)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                new Vector3(leftBoundary, -10, 0),
                new Vector3(leftBoundary, 10, 0)
            );
        }
    }
}