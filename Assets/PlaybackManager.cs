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

    public bool isPlaying = true;

    void Start()
    {
        playBtnText.text = pauseText;

        GetComponent<CanvasRenderer>().SetAlpha(0);
        OnPointerExit(null);
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
        // Hide its own Canvas Renderer
        GetComponent<CanvasRenderer>().SetAlpha(0);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
