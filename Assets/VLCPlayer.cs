using UnityEngine;
using UnityEngine.UI;
using System;
using LibVLCSharp;
using SFB;

/// this class serves as an example on how to configure playback in Unity with VLC for Unity using LibVLCSharp.
/// for libvlcsharp usage documentation, please visit https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/home.md
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
public class VLCPlayer : MonoBehaviour
{


    [SerializeField] Material mat;
    [SerializeField] string videoName;
    [SerializeField] Slider videoSlider;
    LibVLC _libVLC;
    public MediaPlayer _mediaPlayer;
    const int seekTimeDelta = 5000;
    Texture2D tex = null;
    bool playing;
    
    void Awake()
    {
        Core.Initialize(Application.dataPath);

        _libVLC = new LibVLC(enableDebugLogs: true);

        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        //_libVLC.Log += (s, e) => UnityEngine.Debug.Log(e.FormattedLog); // enable this for logs in the editor
        // PlayPause();
    }

    public void OpenVideo()
    {
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false,
            (string[] paths) => { LoadVideo(paths); });
    }

    public void LoadVideo(string[] paths)
    {
        if (paths.Length == 0) return;
        if (playing) Stop();
        videoName = paths[0];
        PlayPause();
    }

    public void Seek(float fraction)
    {
        if (_mediaPlayer == null) return;
        Debug.Log("[VLC] Seeking to " + fraction);
        _mediaPlayer.SetTime((long)(fraction * _mediaPlayer.Length));
    }

    public void SeekForward()
    {
        Debug.Log("[VLC] Seeking forward !");
        _mediaPlayer.SetTime(_mediaPlayer.Time + seekTimeDelta);
    }

    public void SeekBackward()
    {
        Debug.Log("[VLC] Seeking backward !");
        _mediaPlayer.SetTime(_mediaPlayer.Time - seekTimeDelta);
    }

    void OnDisable() 
    {
        _mediaPlayer?.Stop();
        _mediaPlayer?.Dispose();
        _mediaPlayer = null;

        _libVLC?.Dispose();
        _libVLC = null;
    }

    public void PlayPause()
    {
        Debug.Log ("[VLC] Toggling Play Pause !");
        if (_mediaPlayer == null)
        {
            _mediaPlayer = new MediaPlayer(_libVLC);
        }
        if (_mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Pause();
        }
        else
        {
            playing = true;

            if(_mediaPlayer.Media == null)
            {
                // playing remote media
                _mediaPlayer.Media = new Media(videoName);
            }

            _mediaPlayer.Play();
        }
    }

    public void Stop()
    {
        Debug.Log ("[VLC] Stopping Player !");

        playing = false;
        _mediaPlayer?.Stop();
        
        // there is no need to dispose every time you stop, but you should do so when you're done using the mediaplayer and this is how:
        _mediaPlayer?.Dispose(); 
        _mediaPlayer = null;
        mat.mainTexture = null;
        tex = null;
    }

    void Update()
    {
        if(!playing) return;

        if (tex == null)
        {
            // If received size is not null, it and scale the texture
            uint i_videoHeight = 0;
            uint i_videoWidth = 0;

            _mediaPlayer.Size(0, ref i_videoWidth, ref i_videoHeight);
            var texptr = _mediaPlayer.GetTexture(i_videoWidth, i_videoHeight, out bool updated);
            if (i_videoWidth != 0 && i_videoHeight != 0 && updated && texptr != IntPtr.Zero)
            {
                Debug.Log("Creating texture with height " + i_videoHeight + " and width " + i_videoWidth);
                tex = Texture2D.CreateExternalTexture((int)i_videoWidth,
                    (int)i_videoHeight,
                    TextureFormat.RGBA32,
                    false,
                    true,
                    texptr);
                mat.mainTexture = tex;
            }
        }
        else if (tex != null)
        {
            var texptr = _mediaPlayer.GetTexture((uint)tex.width, (uint)tex.height, out bool updated);
            if (updated)
            {
                tex.UpdateExternalTexture(texptr);
            }
        }
        
        if (videoSlider && _mediaPlayer.Length != 0) {
            videoSlider.SetValueWithoutNotify(((float)_mediaPlayer.Time / _mediaPlayer.Length));
        }
    }
}
