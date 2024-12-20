using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SFB;

public class VideoManager : MonoBehaviour
{
    [SerializeField] VideoPlayer video;
    [SerializeField] Slider videoSlider;
    [SerializeField] string videoName;

    void Start()
    {
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false,
            (string[] paths) => { LoadVideo(paths); });
    }



    // Video management
    public void LoadVideo(string[] paths)
    {
        Debug.Log("Loading video...");
        if (paths.Length == 0) return;
        video.url = System.IO.Path.Combine(Application.streamingAssetsPath, paths[0]);
        video.Prepare();
        video.prepareCompleted += Video_prepareCompleted;
    }

    private void Video_prepareCompleted(VideoPlayer source)
    {
        Debug.Log("Video loaded.");
        PlayVideo();

        if (videoSlider == null) return;
        videoSlider.maxValue = (float) video.length;
        videoSlider.minValue = 0;
        videoSlider.value = 0;
    }

    //Methods for Buttons
    public void PlayVideo()
    {
        if (!video.isPrepared) return ;
        video.Play();
    }

    public void PauseVideo()
    {
        if (!video.isPlaying) return;
        video.Pause();
    }

    public void RestartVideo()
    {
        if (!video.isPrepared) return ;
        PauseVideo();
        Seek(0);
    }

    public void Seek(float nTime)
    {
        if(!video.isPrepared) return ;
        nTime = Mathf.Clamp01(nTime);
        video.time = nTime * video.length;
    }

    public void SeekSlider()
    {
        video.time = (videoSlider.value * video.length);
    }
}
