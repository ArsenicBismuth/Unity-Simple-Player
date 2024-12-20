using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using TMPro;

public class PlaybackManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        GetComponent<CanvasRenderer>().SetAlpha(0);
        OnPointerExit(null);
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
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show its own Canvas Renderer
        GetComponent<CanvasRenderer>().SetAlpha(1);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide its own Canvas Renderer
        GetComponent<CanvasRenderer>().SetAlpha(0);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
