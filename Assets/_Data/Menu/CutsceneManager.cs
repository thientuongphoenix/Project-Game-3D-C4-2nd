using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Video")]
    [SerializeField] protected GameObject cutsceneCanvas; // Canvas chứa video player
    [SerializeField] protected VideoPlayer videoPlayer; // VideoPlayer component
    [SerializeField] protected RawImage videoDisplay; // RawImage để hiển thị video
    [SerializeField] protected Button skipButton; // Button để skip cutscene
    [SerializeField] protected TextMeshProUGUI skipButtonText; // Text của skip button
    [SerializeField] protected VideoClip cutsceneVideoClip; // Video clip của cutscene
    
    [Header("Video Settings")]
    [SerializeField] protected int videoWidth = 1920;
    [SerializeField] protected int videoHeight = 1080;
    [SerializeField] protected bool maintainAspectRatio = true;
    [SerializeField] protected float videoScale = 1f;
    [SerializeField] protected bool centerVideo = true; // Căn giữa video trên màn hình
    
    [Header("Scene Settings")]
    [SerializeField] protected string nextSceneName = "MapSelect_Hai"; // Scene tiếp theo sau cutscene
    [SerializeField] protected float delayBeforeNextScene = 0.5f; // Delay trước khi chuyển scene
    
    [Header("Skip Settings")]
    [SerializeField] protected string skipText = "SKIP";
    [SerializeField] protected float skipButtonDelay = 2f; // Delay trước khi hiện skip button
    
    protected virtual void Start()
    {
        this.InitializeCutscene();
        this.SetupVideoPlayer();
        this.StartCutscene();
    }
    
    /// <summary>
    /// Khởi tạo cutscene
    /// </summary>
    protected virtual void InitializeCutscene()
    {
        Debug.Log("=== INITIALIZING CUTSCENE ===");
        
        // Tắt background music khi bắt đầu cutscene
        this.StopBackgroundMusicForCutscene();
        
        // Đảm bảo cutscene canvas được hiển thị
        if (this.cutsceneCanvas != null)
        {
            this.cutsceneCanvas.SetActive(true);
            Debug.Log("Cutscene canvas activated");
        }
        
        // Ẩn cursor trong cutscene
        if (HideMouse.Instance != null)
        {
            HideMouse.Instance.isCursorVisible = false;
            Debug.Log("Cursor hidden for cutscene");
        }
        
        Debug.Log("Cutscene initialization completed!");
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Thiết lập video player
    /// </summary>
    protected virtual void SetupVideoPlayer()
    {
        if (this.videoPlayer == null || this.videoDisplay == null)
        {
            Debug.LogError("VideoPlayer or RawImage is null! Cannot setup video player.");
            return;
        }
        
        Debug.Log("=== SETTING UP VIDEO PLAYER ===");
        
        // Cấu hình VideoPlayer cơ bản
        this.videoPlayer.playOnAwake = false;
        this.videoPlayer.isLooping = false;
        this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        
        // Tạo RenderTexture cho video
        Debug.Log($"Creating RenderTexture with resolution {this.videoWidth}x{this.videoHeight}...");
        RenderTexture renderTexture = new RenderTexture(this.videoWidth, this.videoHeight, 0);
        this.videoPlayer.targetTexture = renderTexture;
        
        // Gán RenderTexture cho RawImage
        this.videoDisplay.texture = renderTexture;
        Debug.Log("RenderTexture assigned to VideoDisplay");
        
        // Thiết lập video clip
        if (this.cutsceneVideoClip != null)
        {
            this.videoPlayer.clip = this.cutsceneVideoClip;
            Debug.Log($"Video clip set: {this.cutsceneVideoClip.name}");
        }
        else
        {
            Debug.LogWarning("Cutscene video clip is null!");
        }
        
        // Thiết lập kích thước video
        this.videoDisplay.rectTransform.sizeDelta = new Vector2(this.videoWidth, this.videoHeight);
        
        // Thiết lập scale
        if (this.videoScale != 1f)
        {
            this.videoDisplay.rectTransform.localScale = Vector3.one * this.videoScale;
        }
        
        // Căn giữa video
        if (this.centerVideo)
        {
            this.videoDisplay.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            this.videoDisplay.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            this.videoDisplay.rectTransform.anchoredPosition = Vector2.zero;
        }
        
        // Thiết lập aspect ratio
        if (this.maintainAspectRatio)
        {
            this.videoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
        }
        
        Debug.Log("Video player setup completed!");
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Bắt đầu cutscene
    /// </summary>
    protected virtual void StartCutscene()
    {
        Debug.Log("=== STARTING CUTSCENE ===");
        
        if (this.videoPlayer != null && this.cutsceneVideoClip != null)
        {
            // Thiết lập event khi video kết thúc
            this.videoPlayer.loopPointReached += this.OnVideoFinished;
            
            // Bắt đầu phát video
            this.videoPlayer.Play();
            Debug.Log("Cutscene video started playing");
            
            // Hiện skip button sau delay
            if (this.skipButtonDelay > 0f)
            {
                Invoke(nameof(ShowSkipButton), this.skipButtonDelay);
            }
            else
            {
                this.ShowSkipButton();
            }
        }
        else
        {
            Debug.LogWarning("VideoPlayer or video clip is null! Skipping cutscene...");
            this.SkipCutscene();
        }
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Hiển thị skip button
    /// </summary>
    protected virtual void ShowSkipButton()
    {
        if (this.skipButton != null)
        {
            this.skipButton.gameObject.SetActive(true);
            Debug.Log("Skip button shown");
        }
        
        if (this.skipButtonText != null)
        {
            this.skipButtonText.text = this.skipText;
        }
    }
    
    /// <summary>
    /// Skip cutscene (gọi từ button)
    /// </summary>
    public virtual void SkipCutscene()
    {
        Debug.Log("=== SKIPPING CUTSCENE ===");
        
        // Dừng video
        if (this.videoPlayer != null)
        {
            this.videoPlayer.Stop();
            Debug.Log("Video stopped");
        }
        
        // Chuyển đến scene tiếp theo
        this.LoadNextScene();
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Xử lý khi video kết thúc
    /// </summary>
    protected virtual void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("=== CUTSCENE VIDEO FINISHED ===");
        
        // Chuyển đến scene tiếp theo
        this.LoadNextScene();
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Load scene tiếp theo
    /// </summary>
    protected virtual void LoadNextScene()
    {
        Debug.Log($"Loading next scene: {this.nextSceneName}");
        
        // Không cần khởi động background music trước khi chuyển scene
        // MapSelectionManager sẽ tự động khởi động nhạc nền khi vào scene
        Debug.Log("Skipping background music start - MapSelectionManager will handle it");
        
        // Chuyển scene với delay
        if (this.delayBeforeNextScene > 0f)
        {
            Invoke(nameof(LoadNextSceneImmediate), this.delayBeforeNextScene);
        }
        else
        {
            this.LoadNextSceneImmediate();
        }
    }
    
    /// <summary>
    /// Load scene tiếp theo ngay lập tức
    /// </summary>
    protected virtual void LoadNextSceneImmediate()
    {
        SceneManager.LoadScene(this.nextSceneName);
    }
    
    /// <summary>
    /// Tắt background music khi bắt đầu cutscene
    /// </summary>
    protected virtual void StopBackgroundMusicForCutscene()
    {
        try
        {
            Debug.Log("=== STOPPING BACKGROUND MUSIC FOR CUTSCENE ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, stopping background music for cutscene...");
                
                // Tắt background music
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Background music stopped for cutscene!");
                }
                else
                {
                    Debug.Log("Background music is already stopped or null");
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot stop background music for cutscene.");
            }
            
            Debug.Log("Background music stop for cutscene completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopBackgroundMusicForCutscene: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Khởi động lại background music sau cutscene
    /// </summary>
    protected virtual void ResumeBackgroundMusicAfterCutscene()
    {
        try
        {
            Debug.Log("=== RESUMING BACKGROUND MUSIC AFTER CUTSCENE ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, resuming background music after cutscene...");
                
                // Khởi động lại background music
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(true);
                    Debug.Log("Background music resumed after cutscene!");
                }
                else
                {
                    Debug.Log("Background music is null, starting new one...");
                    SoundManager.Instance.StartMusicBackground();
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot resume background music after cutscene.");
            }
            
            Debug.Log("Background music resume after cutscene completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ResumeBackgroundMusicAfterCutscene: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Set scene tiếp theo
    /// </summary>
    public virtual void SetNextScene(string sceneName)
    {
        this.nextSceneName = sceneName;
        Debug.Log($"Next scene set to: {this.nextSceneName}");
    }
    
    /// <summary>
    /// Set video clip
    /// </summary>
    public virtual void SetVideoClip(VideoClip clip)
    {
        this.cutsceneVideoClip = clip;
        if (this.videoPlayer != null)
        {
            this.videoPlayer.clip = clip;
        }
        Debug.Log($"Video clip set to: {(clip != null ? clip.name : "null")}");
    }
    
    /// <summary>
    /// Debug method để kiểm tra trạng thái video
    /// </summary>
    [ContextMenu("Debug Video Status")]
    public virtual void DebugVideoStatus()
    {
        Debug.Log("=== DEBUG VIDEO STATUS ===");
        
        // Kiểm tra VideoPlayer
        if (this.videoPlayer == null)
        {
            Debug.LogError("❌ VideoPlayer is NULL!");
        }
        else
        {
            Debug.Log($"✅ VideoPlayer: {this.videoPlayer.name}");
            Debug.Log($"   - Clip: {(this.videoPlayer.clip != null ? this.videoPlayer.clip.name : "NULL")}");
            Debug.Log($"   - Render Mode: {this.videoPlayer.renderMode}");
            Debug.Log($"   - Target Texture: {(this.videoPlayer.targetTexture != null ? this.videoPlayer.targetTexture.name : "NULL")}");
            Debug.Log($"   - Is Playing: {this.videoPlayer.isPlaying}");
            Debug.Log($"   - Play On Awake: {this.videoPlayer.playOnAwake}");
            Debug.Log($"   - Is Looping: {this.videoPlayer.isLooping}");
        }
        
        // Kiểm tra RawImage
        if (this.videoDisplay == null)
        {
            Debug.LogError("❌ VideoDisplay RawImage is NULL!");
        }
        else
        {
            Debug.Log($"✅ VideoDisplay: {this.videoDisplay.name}");
            Debug.Log($"   - Texture: {(this.videoDisplay.texture != null ? this.videoDisplay.texture.name : "NULL")}");
            Debug.Log($"   - Size Delta: {this.videoDisplay.rectTransform.sizeDelta}");
            Debug.Log($"   - Local Scale: {this.videoDisplay.rectTransform.localScale}");
            Debug.Log($"   - Anchored Position: {this.videoDisplay.rectTransform.anchoredPosition}");
        }
        
        // Kiểm tra Canvas
        if (this.cutsceneCanvas == null)
        {
            Debug.LogError("❌ CutsceneCanvas is NULL!");
        }
        else
        {
            Debug.Log($"✅ CutsceneCanvas: {this.cutsceneCanvas.name}");
            Debug.Log($"   - Active: {this.cutsceneCanvas.activeSelf}");
        }
        
        // Kiểm tra Video Clip
        if (this.cutsceneVideoClip == null)
        {
            Debug.LogError("❌ CutsceneVideoClip is NULL!");
        }
        else
        {
            Debug.Log($"✅ CutsceneVideoClip: {this.cutsceneVideoClip.name}");
            Debug.Log($"   - Length: {this.cutsceneVideoClip.length} seconds");
        }
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Method để test video playback
    /// </summary>
    [ContextMenu("Test Video Playback")]
    public virtual void TestVideoPlayback()
    {
        Debug.Log("=== TESTING VIDEO PLAYBACK ===");
        
        if (this.videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is null! Cannot test playback.");
            return;
        }
        
        if (this.cutsceneVideoClip == null)
        {
            Debug.LogError("Video clip is null! Cannot test playback.");
            return;
        }
        
        // Thiết lập lại video player
        this.SetupVideoPlayer();
        
        // Bắt đầu phát video
        this.videoPlayer.Play();
        Debug.Log("Video playback test started!");
        
        // Kiểm tra sau 1 giây
        Invoke(nameof(CheckVideoPlaybackStatus), 1f);
    }
    
    /// <summary>
    /// Kiểm tra trạng thái phát video
    /// </summary>
    protected virtual void CheckVideoPlaybackStatus()
    {
        if (this.videoPlayer != null)
        {
            Debug.Log($"Video playback status after 1 second:");
            Debug.Log($"   - Is Playing: {this.videoPlayer.isPlaying}");
            Debug.Log($"   - Frame: {this.videoPlayer.frame}");
            Debug.Log($"   - Time: {this.videoPlayer.time}");
            
            if (!this.videoPlayer.isPlaying)
            {
                Debug.LogWarning("Video is not playing! Check video file format and codec support.");
            }
        }
    }
}
