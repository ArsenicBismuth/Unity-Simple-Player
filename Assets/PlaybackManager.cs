using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using TMPro;

public class PlaybackManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject videoManager;
    [SerializeField] TextMeshProUGUI playBtnText;

    public string playText;
    public string pauseText;

    VLCPlayer vlcPlayer;

    void Start()
    {
        vlcPlayer = videoManager.GetComponent<VLCPlayer>();
        playBtnText.text = pauseText;

        GetComponent<CanvasRenderer>().SetAlpha(0);
        OnPointerExit(null);
    }

    public void OnClick()
    {
        if (vlcPlayer._mediaPlayer.IsPlaying)
        {
            vlcPlayer.PlayPause();
            playBtnText.text = playText;
        }
        else
        {
            vlcPlayer.PlayPause();
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
