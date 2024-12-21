using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Video;
using SFB;

public class NativePlayer : MonoBehaviour
{
    public VideoPlayer video;
    [SerializeField] Slider videoSlider;
    [SerializeField] string videoName;
    [SerializeField] UnityEvent playFunc;
    [SerializeField] UnityEvent pauseFunc;
    
    
    private void Update()
    {
        if (!video.isPrepared) return;
        if (!videoSlider) return;
        videoSlider.SetValueWithoutNotify((float)(video.time / video.length));
    }



    // Video management
    public void OpenVideo()
    {
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false,
            (string[] paths) => { LoadVideo(paths); });
    }
    public void LoadVideo(string[] paths)
    {
        Debug.Log("Loading video...");
        if (paths.Length == 0) return;
        video.url = System.IO.Path.Combine(Application.streamingAssetsPath, paths[0]);
        video.Prepare();
        video.prepareCompleted += Video_prepareCompleted;
        video.isLooping = true;
    }

    private void Video_prepareCompleted(VideoPlayer source)
    {
        Debug.Log("Video loaded.");
        Play();
    }

    //Methods for Buttons
    public void Play()
    {
        if (!video.isPrepared) return ;
        video.Play();
        playFunc.Invoke();
    }

    public void Pause()
    {
        if (!video.isPlaying) return;
        video.Pause();
        pauseFunc.Invoke();
    }

    public void Restart()
    {
        if (!video.isPrepared) return ;
        Pause();
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
