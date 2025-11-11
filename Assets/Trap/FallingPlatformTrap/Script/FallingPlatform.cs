using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f; // Jeda sebelum jatuh
    [SerializeField] private float destroyDelay = 3f; // Waktu sebelum hancur

    private bool isFalling = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            // Cek jika player mendarat di atas
            if (collision.contacts[0].normal.y < -0.5) 
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;

        // (Opsional) Tambahkan efek goyang (shake) di sini

        yield return new WaitForSeconds(fallDelay);

        // Matikan kinematic agar platform jatuh kena gravitasi
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Hancurkan platform setelah beberapa detik
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}