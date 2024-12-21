using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using LibVLCSharp;
using SFB;
using System.Threading.Tasks;

/// this class serves as an example on how to configure playback in Unity with VLC for Unity using LibVLCSharp.
/// for libvlcsharp usage documentation, please visit https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/home.md
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
public class VLCPlayer : MonoBehaviour
{

    // NOTE: Due to the internal LibVLCSharp implementation, videos with 180 or VR metadata will not be displayed correctly.
    // As they assume a 2D mesh. Meanwhile untagged 360 videos will be displayed correctly.
    // Issues:
    // - https://code.videolan.org/videolan/vlc/-/issues/26907
    // - https://code.videolan.org/videolan/vlc-unity/-/issues/166
    
    [SerializeField] Material mat;
    [SerializeField] string videoName;
    [SerializeField] Slider videoSlider;
    LibVLC _libVLC;
    public MediaPlayer _mediaPlayer;
    const int seekTimeDelta = 5000;
    Texture2D tex = null;
    bool playing;
    bool is360;

    [SerializeField] UnityEvent playFunc;
    [SerializeField] UnityEvent pauseFunc;

    public float Yaw = 0;
    public float Pitch = 0;
    public float Roll = 0;
    public float fov = 360;
    
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
            Pause();
        }
        else
        {
            playing = true;

            if(_mediaPlayer.Media == null)
            {
                // playing remote media
                _mediaPlayer.Media = new Media(videoName);
                Task.Run(() => CheckMeta(_mediaPlayer.Media));
            }

            Play();
        }
    }

    public void Play() {
        _mediaPlayer.Play();
        playFunc.Invoke();
    }

    public void Pause() {
        _mediaPlayer.Pause();
        pauseFunc.Invoke();
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

    public void SetVolume(float volume)
    {
        _mediaPlayer.SetVolume((int)(volume * 100));
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

    async void CheckMeta(Media media)
    {
        var result = await media.ParseAsync(_libVLC, MediaParseOptions.ParseNetwork);
        var trackList = media.TrackList(TrackType.Video);
        is360 = trackList[0].Data.Video.Projection == VideoProjection.Equirectangular;
        Debug.Log(trackList[0].Data.Video);
        
        if(is360) {
            Debug.Log("The video is a 360 video, adjusting the viewport.");
            UpdateViewport();
        } else {
            Debug.Log("The video is not a 360, no adjustments will be made");
        }

        trackList.Dispose();
    }

    void OnGUI()
    {
        if(is360) {
            UpdateViewport();
        }
    }

    void UpdateViewport() {
        _mediaPlayer.UpdateViewpoint(Yaw, Pitch, Roll, fov);
    }
}
