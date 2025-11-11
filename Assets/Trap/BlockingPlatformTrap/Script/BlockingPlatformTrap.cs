using UnityEngine;

public class BlockingTrap : MonoBehaviour
{
    [SerializeField] private GameObject blockingPlatform;

    private void Start()
    {
        // Pastikan penghalang mati di awal
        if (blockingPlatform != null)
        {
            blockingPlatform.SetActive(false);
        }
    }

    // Saat player masuk zona trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (blockingPlatform != null)
            {
                blockingPlatform.SetActive(true);
            }
        }
    }

    // Saat player keluar zona trigger (agar bisa di-reset)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (blockingPlatform != null)
            {
                blockingPlatform.SetActive(false);
            }
        }
    }
}