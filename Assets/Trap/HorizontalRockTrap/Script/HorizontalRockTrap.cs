using System.Collections;
using UnityEngine;

public class HorizontalRockTrap : MonoBehaviour
{
    [SerializeField] private GameObject rockObject;
    [SerializeField] private float moveSpeed = 10f; // Kecepatan batu meluncur
    [SerializeField] private float destroyDelay = 4f; // Waktu sebelum batu hancur

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            
            // Nonaktifkan trigger ini
            GetComponent<Collider2D>().enabled = false;

            // Aktifkan batu
            if (rockObject != null)
            {
                rockObject.SetActive(true);
                Rigidbody2D rockRb = rockObject.GetComponent<Rigidbody2D>();
                if (rockRb != null)
                {
                    rockRb.isKinematic = false;
                    // Beri kecepatan horizontal (ke kanan)
                    // Ganti 'moveSpeed' jadi '-moveSpeed' jika ingin ke kiri
                    rockRb.velocity = new Vector2(moveSpeed, 0); 
                }

                // Hancurkan batu setelah beberapa detik
                StartCoroutine(DestroyObject(rockObject, destroyDelay));
            }
        }
    }

    private IEnumerator DestroyObject(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
        }
        // Hancurkan trigger ini juga
        Destroy(gameObject);
    }
}