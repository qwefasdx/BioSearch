using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    public Canvas targetCanvas;
    public TMP_InputField targetInputField;

    private Camera activeCamera;
    private bool isSwitching = false;

    //  View ����
    private enum ViewMode { Front, Left, Right }
    private ViewMode currentView = ViewMode.Front;

    //  W Ÿ�̸� ����
    private float wTimer = 0f;
    private bool wTimerActive = false;
    public float switchDelay = 1.5f; // ��ȯ ��� �ð�

    void Start()
    {
        SetCameraState(camera1, true);
        SetCameraState(camera2, false);
        activeCamera = camera1;
        UpdateCanvasRaycast();
        currentView = ViewMode.Front; // �⺻ ���� ����
    }

    void Update()
    {
        if (isSwitching) return;
        if (targetInputField != null && targetInputField.isFocused) return;

        //  camera1 + Front ������ ���� W ��ȯ ����
        if (activeCamera == camera1 && currentView == ViewMode.Front)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                wTimer = switchDelay;
                wTimerActive = true;
            }

            if (wTimerActive)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    wTimerActive = false; // Ÿ�̸� ���
                    return;
                }

                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S))
                {
                    return; // �ٸ� Ű ������ �Ͻ�����
                }

                wTimer -= Time.deltaTime;

                if (wTimer <= 0f)
                {
                    StartCoroutine(SwitchFrom1To2());
                    wTimerActive = false;
                }
            }
        }

        //  camera2 ���¿��� S�� ����
        if (activeCamera == camera2 && Input.GetKeyDown(KeyCode.S))
        {
            SwitchFrom2To1();
        }

        //  A, D�� ���� ��ȯ (camera1 ���¿�����)
        if (activeCamera == camera1)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentView = ViewMode.Left;
                // ī�޶� �̵�/ȸ�� ���� �߰�
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                currentView = ViewMode.Right;
                // ī�޶� �̵�/ȸ�� ���� �߰�
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                currentView = ViewMode.Front; // ���� ����
                // ī�޶� �̵�/ȸ�� ���� ���� �߰�
            }
        }
    }

    IEnumerator SwitchFrom1To2()
    {
        isSwitching = true;

        yield return null;

        SetCameraState(camera1, false);
        SetCameraState(camera2, true);
        activeCamera = camera2;
        UpdateCanvasRaycast();

        isSwitching = false;
    }

    void SwitchFrom2To1()
    {
        wTimerActive = false; // W Ÿ�̸� ���
        SetCameraState(camera2, false);
        SetCameraState(camera1, true);
        activeCamera = camera1;
        currentView = ViewMode.Front; // ���� �� ����
        UpdateCanvasRaycast();
    }

    private void SetCameraState(Camera cam, bool state)
    {
        if (cam != null)
        {
            cam.enabled = state;
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null)
                listener.enabled = state;
        }
    }

    private void UpdateCanvasRaycast()
    {
        if (targetCanvas != null)
        {
            GraphicRaycaster gr = targetCanvas.GetComponent<GraphicRaycaster>();
            if (gr != null)
                gr.enabled = (activeCamera == camera2);
        }
    }
}
