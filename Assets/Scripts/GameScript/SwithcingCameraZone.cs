using UnityEngine;

public class SwitchingCameraZone : MonoBehaviour
{
    // 1. Kamera yang akan AKTIF jika player masuk zona INI
    [SerializeField] private GameObject cameraToActivate;

    // 2. Buat Tag baru di Unity bernama "StageCamera"
    // (Pastikan nama tag ini persis sama)
    private string cameraTag = "StageCamera";

    // Kita HANYA butuh OnTriggerEnter2D.
    // Kita tidak perlu OnTriggerExit2D sama sekali.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah itu Player
        if (other.CompareTag("Player"))
        {
            // LANGKAH 1: Cari SEMUA kamera di scene yang punya tag "StageCamera"
            GameObject[] allCameras = GameObject.FindGameObjectsWithTag(cameraTag);
            
            // LANGKAH 2: Matikan SEMUA kamera tersebut
            foreach (GameObject cam in allCameras)
            {
                cam.SetActive(false);
            }

            // LANGKAH 3: Aktifkan HANYA kamera yang spesifik untuk zona INI
            if (cameraToActivate != null)
            {
                cameraToActivate.SetActive(true);
            }
        }
    }
}