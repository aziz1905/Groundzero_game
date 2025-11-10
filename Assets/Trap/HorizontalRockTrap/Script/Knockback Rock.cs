using UnityEngine;

public class KnockbackTrap : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 15f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null)
            {
                // Hitung arah dorongan (menjauh dari batu)
                // '1' di 'x' berarti dorong ke kanan, '-1' ke kiri
                // '0.5f' di 'y' agar sedikit terangkat
                float direction = (collision.transform.position.x > transform.position.x) ? 1 : -1;
                Vector2 knockbackDirection = new Vector2(direction, 0.5f).normalized;

                // Hentikan kecepatan player & dorong
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}