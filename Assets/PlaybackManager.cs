using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class PlaybackManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject videoManager;
    [SerializeField] TextMeshProUGUI playBtnText;

    public string playText;
    public string pauseText;
    [SerializeField] UnityEvent playFunc;
    [SerializeField] UnityEvent pauseFunc;
    [SerializeField] UnityEvent openVideoFunc;

    bool isPlaying = false;

    void Start()
    {
        playBtnText.text = playText;
    }

    public void OnOpenVideo()
    {
        openVideoFunc.Invoke();
        isPlaying = true;
        playBtnText.text = pauseText;
    }

    public void OnClick()
    {
        if (isPlaying)
        {
            Debug.Log("Pause");
            pauseFunc.Invoke();
            playBtnText.text = playText;
        }
        else
        {
            Debug.Log("Play");
            playFunc.Invoke();
            playBtnText.text = pauseText;
        }
        isPlaying = !isPlaying;
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
        if (!isPlaying) return; // Only hide on play

        // Hide its own Canvas Renderer
        GetComponent<CanvasRenderer>().SetAlpha(0);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
