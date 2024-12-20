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
    
    private void Update()
    {
        if (!video.isPrepared) return;
        if (!videoSlider) return;
        videoSlider.SetValueWithoutNotify((float)(video.time / video.length));
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

    public void Seek(float fraction)
    {
        Debug.Log("Seeking...");
        if(!video.isPrepared) return ;
        fraction = Mathf.Clamp01(fraction);
        video.time = fraction * video.length;
    }

    public void SetVolume(float volume)
    {
        video.SetDirectAudioVolume(0, volume);
        if (volume != 0) 
        {
            video.SetDirectAudioMute(0, false);
        }
    }
}
