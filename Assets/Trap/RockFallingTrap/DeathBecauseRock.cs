using UnityEngine;

public class RockFalling : MonoBehaviour
{
    // Pastikan objek trap ini punya Collider2D dengan "Is Trigger" = true

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah objek solid yang kita tabrak punya tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ambil skrip 'Movement' dari player
            PlayerController playerMovement = collision.gameObject.GetComponent<PlayerController>();

            if (playerMovement != null)
            {
                // Panggil fungsi respawn-nya
                playerMovement.DieAndRespawn();
            }
        }
    }
}