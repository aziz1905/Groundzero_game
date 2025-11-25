using UnityEngine;

public class BlockingTrap : MonoBehaviour
{
    [SerializeField] private GameObject blockingPlatform;
    [SerializeField] private float upwardVelocityThreshold = 0.05f;

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
        if (!IsPlayerBody(other, out Rigidbody2D playerRb))
            return;

        if (playerRb.velocity.y < upwardVelocityThreshold)
            return; // Only trigger when the player is moving upward

        if (blockingPlatform != null)
        {
            blockingPlatform.SetActive(true);
        }
    }

    // Saat player keluar zona trigger (agar bisa di-reset)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayerBody(other, out _))
            return;

        if (blockingPlatform != null)
        {
            blockingPlatform.SetActive(false);
        }
    }

    private bool IsPlayerBody(Collider2D other, out Rigidbody2D playerRb)
    {
        playerRb = null;
        Transform root = other.transform.root;
        if (root == null || !root.CompareTag("Player"))
            return false;

        playerRb = root.GetComponent<Rigidbody2D>();
        return playerRb != null;
    }
}