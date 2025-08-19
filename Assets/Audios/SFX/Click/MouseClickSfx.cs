// GlobalMouseClickSfxExposed.cs
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MouseClickSfx : MonoBehaviour
{
    [Header("클릭 사운드")]
    [Tooltip("클릭 시 재생할 클립")]
    public AudioClip clickClip;

    [Tooltip("좌클릭만 감지할지 여부 (false면 우/중클릭도 포함)")]
    public bool leftClickOnly = true;

    [Header("Mixer 라우팅")]
    [Tooltip("출력할 AudioMixerGroup (예: UI 또는 SFX 그룹)")]
    public AudioMixerGroup outputGroup;

    [Header("볼륨/피치 설정")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    [Tooltip("피치 랜덤 가감 범위 (±)")]
    [Range(0f, 0.2f)] public float pitchVariance = 0.03f;

    [Header("스팸 억제")]
    [Tooltip("연속 클릭 시 최소 간격 (밀리초). 0이면 제한 없음")]
    [Range(0, 200)] public int minIntervalMs = 40;

    private AudioSource source;
    private double lastPlayDsp = -1;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D 고정 (전역 사운드)
        if (outputGroup) source.outputAudioMixerGroup = outputGroup;
    }

    void Update()
    {
        if (leftClickOnly)
        {
            if (Input.GetMouseButtonDown(0)) TryPlay();
        }
        else
        {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2))
                TryPlay();
        }
    }

    void TryPlay()
    {
        if (clickClip == null) return;

        // 스팸 방지
        if (minIntervalMs > 0 && lastPlayDsp > 0)
        {
            double dtMs = (AudioSettings.dspTime - lastPlayDsp) * 1000.0;
            if (dtMs < minIntervalMs) return;
        }
        lastPlayDsp = AudioSettings.dspTime;

        // 피치 랜덤 적용
        float prevPitch = source.pitch;
        source.pitch = Mathf.Clamp(pitch + Random.Range(-pitchVariance, pitchVariance), 0.5f, 2f);

        // 재생
        source.PlayOneShot(clickClip, volume);

        // 피치 복구
        source.pitch = prevPitch;
    }
}
