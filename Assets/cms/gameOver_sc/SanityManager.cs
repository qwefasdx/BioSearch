using UnityEngine;
using TMPro;

public class SanityManager : MonoBehaviour
{
    [Header("정신력 설정")]
    public float maxSanity = 100f;
    private float currentSanity;

    [Header("UI 표시 (임시 TMP)")]
    public TextMeshProUGUI sanityText;

    [Header("감소 설정")]
    public float decreaseAmount = 5f;

    private bool isGameOverTriggered = false;

    void Start()
    {
        currentSanity = maxSanity;
        UpdateSanityUI();
    }

    void Update()
    {
        if (isGameOverTriggered)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseSanity(decreaseAmount);
        }
    }

    public void DecreaseSanity(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);
        UpdateSanityUI();

        if (currentSanity <= 0f)
        {
            isGameOverTriggered = true;
            OnSanityDepleted();
        }
    }

    void UpdateSanityUI()
    {
        if (sanityText != null)
            sanityText.text = $"Sanity: {currentSanity:0} / {maxSanity:0}";
    }

    void OnSanityDepleted()
    {
        //  GameOverManager에게 알림을 보냄
        FindObjectOfType<GameOverManager>()?.TriggerGameOver("정신력 0으로 인한 게임 오버");
    }

    public void ResetSanity()
    {
        currentSanity = maxSanity;
        isGameOverTriggered = false;
        UpdateSanityUI();
    }
}
