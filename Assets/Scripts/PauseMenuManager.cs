using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
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
    public Slider bgmSlider;           // 볼륨 슬라이더
    public AudioMixer audioMixer;      // 오디오 믹서 연결용

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // 버튼 이벤트 연결
        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(RestartGame);
        quitButton?.onClick.AddListener(QuitGame);

        // 슬라이더 값 저장 및 불러오기
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
        Time.timeScale = 0f; // 게임 멈춤
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI?.SetActive(false);
        Time.timeScale = 1f; // 게임 재생
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
        // AudioMixer에 "BGMVolume" 파라미터 연결되어 있어야 함
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
