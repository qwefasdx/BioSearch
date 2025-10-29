using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;     // ESC ������ �ߴ� �г�

    [Header("Buttons")]
    public Button resumeButton;        // �ٽ� ���
    public Button restartButton;       // �ٽ� ����
    public Button quitButton;          // ����

    [Header("Audio Settings")]
    public Slider bgmSlider;           // BGm ���� �����̴�
    public AudioSource bgmSource;      //  BGM ������Ʈ�� AudioSource ����
    public GameObject soundManager;    // (�ʿ��) ���� �Ŵ��� ����

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // ��ư ����
        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(RestartGame);
        quitButton?.onClick.AddListener(QuitGame);

        // ���� �����̴� �ʱ�ȭ
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
            bgmSource.volume = value; // BGM ���� ���� ����
        }

        // �ʿ��ϴٸ� SoundManager���� �ݿ�
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
        Debug.Log("���� �޴��� �̵��մϴ�.");
        SceneManager.LoadScene(0);
    }
}

