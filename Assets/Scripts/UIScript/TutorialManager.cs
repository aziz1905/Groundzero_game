using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float showDuration = 5f; // Berapa lama tampil

    [Header("Key References")]
    // Kita tarik Prefab yang sudah jadi ke sini nanti
    [SerializeField] private TutorialKeyUI keyLeft;   // Tombol A
    [SerializeField] private TutorialKeyUI keyRight;  // Tombol D
    [SerializeField] private TutorialKeyUI keyJump;   // Tombol Spasi

    private CanvasGroup canvasGroup;
    private bool isActive = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; // Sembunyi di awal
    }

    private void Start()
    {
        // Setup huruf di awal biar pasti
        if(keyLeft) keyLeft.SetKeyName("A");
        if(keyRight) keyRight.SetKeyName("D");
        if(keyJump) keyJump.SetKeyName("SPACE");
    }

    private void Update()
    {
        if (!isActive) return;

        // Cek Input Pemain & Update Visual Tombol
        // A (Kiri)
        if (keyLeft != null) 
            keyLeft.SetPressedState(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));

        // D (Kanan)
        if (keyRight != null) 
            keyRight.SetPressedState(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow));

        // Spasi (Lompat)
        if (keyJump != null) 
            keyJump.SetPressedState(Input.GetKey(KeyCode.Space));
    }

    public void ShowTutorial()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateUI());
    }

    private IEnumerator AnimateUI()
    {
        isActive = true;

        // Fade In (Muncul)
        yield return StartCoroutine(Fade(0, 1, 0.5f));

        // Tunggu pemain baca
        yield return new WaitForSeconds(showDuration);

        // Fade Out (Hilang)
        yield return StartCoroutine(Fade(1, 0, 0.5f));

        isActive = false;
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        float t = 0;
        while(t < time)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, t / time);
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}