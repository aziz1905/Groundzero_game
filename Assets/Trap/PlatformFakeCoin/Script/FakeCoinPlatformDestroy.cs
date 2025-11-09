using UnityEngine;

public class FakeCoinPlatformDestroy : MonoBehaviour
{
    [SerializeField] private GameObject Coin;
    [SerializeField] private GameObject Platform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player entered trigger!");
            Platform.SetActive(false);
        }
        Destroy(Coin);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("❌ Player exited trigger!");
            Platform.SetActive(false);
        }
    }
}
