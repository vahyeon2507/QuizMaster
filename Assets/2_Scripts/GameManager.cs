using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI / Flow")]
    [SerializeField] private Quiz quiz;
    [SerializeField] private EndScreen endScreen;
    [SerializeField] private GameObject loadingCanvas;
    public static GameManager Instance;

    [Header("BGM (Audio)")]
    [SerializeField] private AudioClip bgmClip;          // 인스펙터에 BGM 파일 연결
    [SerializeField] private bool playBgmOnStart = true; // 시작할 때 자동 재생 여부
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 0.6f;
    [SerializeField] private bool loopBgm = true;

    private AudioSource bgmSource;
    private bool isMuted = false;

    private void Awake()
    {
        // 싱글톤 인스턴스가 중복 생성되지 않도록 처리
        if (Instance == null)
        {
            Instance = this;
            // 만약 BGM을 씬 전환에도 끊기지 않게 유지하려면 아래 주석을 해제하세요.
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        EnsureAudioSource();
    }

    private void Start()
    {
        // 필요하면 자동 재생
        if (playBgmOnStart)
        {
            PlayBGM();
        }
    }

    private void EnsureAudioSource()
    {
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.playOnAwake = false;
        bgmSource.loop = loopBgm;
        bgmSource.volume = Mathf.Clamp01(bgmVolume);

        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
        }
    }

    #region BGM API
    public void PlayBGM()
    {
        if (bgmSource == null) EnsureAudioSource();

        if (bgmSource.clip == null)
        {
            if (bgmClip == null)
            {
                Debug.LogWarning("[GameManager] PlayBGM 호출했으나 bgmClip이 할당되어 있지 않습니다.");
                return;
            }
            bgmSource.clip = bgmClip;
        }

        if (!bgmSource.isPlaying && !isMuted)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }

    public void ToggleMuteBGM()
    {
        isMuted = !isMuted;
        if (bgmSource == null) EnsureAudioSource();

        if (isMuted)
        {
            bgmSource.Pause();
        }
        else
        {
            if (bgmSource.clip != null)
                bgmSource.UnPause();
            else
                PlayBGM();
        }
    }

    public bool IsBgmPlaying()
    {
        return bgmSource != null && bgmSource.isPlaying;
    }
    #endregion

    public void ShowQuizSceen()
    {
        quiz.gameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
        loadingCanvas.SetActive(false);
    }

    public void ShowEndScreen()
    {
        quiz.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(true);
        endScreen.ShowFinalScore();
        loadingCanvas.SetActive(false);
    }

    public void ShowLoadingScreen()
    {
        quiz.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(false);
        loadingCanvas.SetActive(true);
    }

    public void OnReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
