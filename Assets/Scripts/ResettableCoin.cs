using UnityEngine;

// Script ini membuat coin bisa di-reset saat player respawn
public class ResettableCoin : MonoBehaviour
{
    private Vector3 initialPosition;
    private bool isCollected = false;

    private void Start()
    {
        initialPosition = transform.position;
        GameManager.OnPlayerRespawn += ResetCoin;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerRespawn -= ResetCoin;
    }

    // Fungsi ini dipanggil saat coin di-collect
    public void CollectCoin()
    {
        isCollected = true;
        gameObject.SetActive(false); 
    }

    private void ResetCoin()
    {
        Debug.Log($"ResetCoin dipanggil untuk: {gameObject.name}"); 
        
        // Kembalikan coin ke posisi awal
        transform.position = initialPosition;
        isCollected = false;
        gameObject.SetActive(true); // Show lagi
    }
}