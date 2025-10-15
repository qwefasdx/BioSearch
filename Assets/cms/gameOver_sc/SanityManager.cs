using UnityEngine;
using TMPro;

public class SanityManager : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    private float currentSanity;

    [Header("UI (TMP Text)")]
    public TextMeshProUGUI sanityText;

    [Header("Decrease Settings")]
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

        // For test: decrease sanity when pressing the left arrow key
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
        GameOverManager gameOver = FindObjectOfType<GameOverManager>();
        if (gameOver != null)
        {
            gameOver.TriggerGameOver("Sanity reached zero");
        }
    }

    public void ResetSanity()
    {
        currentSanity = maxSanity;
        isGameOverTriggered = false;
        UpdateSanityUI();
    }
}
