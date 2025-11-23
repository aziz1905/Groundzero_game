using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialUI;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pastikan yang lewat adalah Player
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (tutorialUI != null)
            {
                // 1. Panggil UI untuk muncul
                tutorialUI.ShowTutorial();
                
                // 2. Tandai sudah terpicu
                hasTriggered = true;
                
                // 3. MATIKAN COLLIDER (Agar trigger ini hilang/mati selamanya)
                GetComponent<Collider2D>().enabled = false; 
            }
        }
    }
}