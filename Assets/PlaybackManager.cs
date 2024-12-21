using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class PlaybackManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject videoManager;
    [SerializeField] TextMeshProUGUI playBtnText;
    [SerializeField] GameObject InfoPanel;

    public string playText;
    public string pauseText;
    [SerializeField] UnityEvent playFunc;
    [SerializeField] UnityEvent pauseFunc;
    [SerializeField] UnityEvent openVideoFunc;
    [SerializeField] UnityEvent<float> seekFunc;
    [SerializeField] UnityEvent<float> volumeFunc;

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
        Destroy(InfoPanel);
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

    public void OnSeek(float fraction)
    {
        seekFunc.Invoke(fraction);
    }

    public void OnVolume(float volume)
    {
        volumeFunc.Invoke(volume);
    }
    

    // Panel handling
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

    public void ToggleScene()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int nextSceneIndex = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string sceneName = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
