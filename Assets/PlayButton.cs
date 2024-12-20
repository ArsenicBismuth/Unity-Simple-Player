using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class PlaybackManager : MonoBehaviour
{
    [SerializeField] GameObject videoManagerObj;
    [SerializeField] TextMeshProUGUI playBtnText;

    public string playText;
    public string pauseText;

    VideoManager videoManager;

    void Start()
    {
        videoManager = videoManagerObj.GetComponent<VideoManager>();
        playBtnText.text = pauseText;
    }

    public void OnClick()
    {
        if (videoManager.video.isPlaying)
        {
            videoManager.PauseVideo();
            playBtnText.text = playText;
        }
        else
        {
            videoManager.PlayVideo();
            playBtnText.text = pauseText;
        }
    }
}
