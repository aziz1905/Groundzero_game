using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounceForce = 30f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Cek apakah player mendarat di atas (bukan dari samping)
            if (collision.contacts[0].normal.y < -0.5) 
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Reset kecepatan vertikal player (agar pantulan konsisten)
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                    
                    // Tambahkan dorongan ke atas
                    playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                    // Anda bisa tambahkan suara "boing" di sini
                }
            }
        }
    }
}