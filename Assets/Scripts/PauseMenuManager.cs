using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;     // ESC 누르면 뜨는 패널

    [Header("Buttons")]
    public Button resumeButton;        // 다시 재생
    public Button restartButton;       // 다시 시작
    public Button quitButton;          // 종료

    [Header("Audio Settings")]
    public Slider bgmSlider;           // BGm 볼륨 슬라이더
    public AudioSource bgmSource;      //  BGM 오브젝트의 AudioSource 연결
    public GameObject soundManager;    // (필요시) 사운드 매니저 연결

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // 버튼 연결
        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(RestartGame);
        quitButton?.onClick.AddListener(QuitGame);

        // 볼륨 슬라이더 초기화
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);

            if (PlayerPrefs.HasKey("BGMVolume"))
            {
                float savedVol = PlayerPrefs.GetFloat("BGMVolume");
                bgmSlider.value = savedVol;
                SetBGMVolume(savedVol);
            }
            else
            {
                bgmSlider.value = 1f;
                SetBGMVolume(1f);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI?.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI?.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value; // BGM 볼륨 직접 제어
        }

        // 필요하다면 SoundManager에도 반영
        if (soundManager != null)
        {
            AudioSource[] allSources = soundManager.GetComponentsInChildren<AudioSource>();
            foreach (var src in allSources)
            {
                if (src.gameObject.name.ToLower().Contains("bgm"))
                    src.volume = value;
            }
        }

        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("메인 메뉴로 이동합니다.");
        SceneManager.LoadScene(0);
    }
}

