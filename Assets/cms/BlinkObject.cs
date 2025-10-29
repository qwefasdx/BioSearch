using UnityEngine;
using System.Collections;

public class BlinkObject : MonoBehaviour
{
    private Renderer rend;
    public Color blinkColor = Color.black; // ������ ��
    public float blinkDuration = 0.1f;   // ������ �ð�
    public int blinkCount = 1;           // �� �� ��������

    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void StartBlink()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            rend.material.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);
            rend.material.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
