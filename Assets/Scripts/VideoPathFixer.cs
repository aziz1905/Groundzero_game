using UnityEngine;
using UnityEngine.Video;

public class VideoPathFixer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string namaFileVideo; 

    void Start()
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, namaFileVideo);
        
        // Paksa video player pakai mode URL
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        
        // Play!
        videoPlayer.Play();
    }
}