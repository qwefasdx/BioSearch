using UnityEngine;

public class ObjectClick : MonoBehaviour
{
    public Camera targetCamera;  // ����� ī�޶� ���� ����
    public UIManager uiManager;  // UI �Ŵ��� ����

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            // Camera.main ��� targetCamera ���
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                BlinkObject blink = hit.collider.GetComponent<BlinkObject>();
                if (blink != null)
                {
                    blink.StartBlink();
                    //uiManager.DecreaseCounter();
                }
            }
        }
    }
}
