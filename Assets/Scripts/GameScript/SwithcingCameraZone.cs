using UnityEngine;

public class SwitchingCameraZone : MonoBehaviour
{
    [SerializeField] private GameObject cameraToActivate;
    private const string CameraTag = "StageCamera";

    public static void ActivateStageCamera(GameObject targetCamera)
    {
        if (targetCamera == null) return;

        GameObject[] allCameras = GameObject.FindGameObjectsWithTag(CameraTag);
        foreach (GameObject cam in allCameras)
        {
            cam.SetActive(false);
        }

        targetCamera.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ActivateStageCamera(cameraToActivate);
    }
}