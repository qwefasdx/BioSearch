using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP_InputField 사용
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;   // 1번 카메라
    public Camera camera2;   // 2번 카메라

    public Canvas targetCanvas;      // 클릭 제한을 적용할 Canvas
    public TMP_InputField targetInputField; // 입력 감지할 TMP_InputField

    private Camera activeCamera;
    private bool isSwitching = false;

    void Start()
    {
        // 시작 시 1번 카메라 활성화, 2번은 비활성화
        SetCameraState(camera1, true);
        SetCameraState(camera2, false);
        activeCamera = camera1;

        // 1번 카메라에서 UI 비활성화
        UpdateCanvasRaycast();
    }

    void Update()
    {
        if (isSwitching) return;

        // TMP_InputField에 포커스가 있을 때는 키 입력 무시
        if (targetInputField != null && targetInputField.isFocused)
            return;

        if (activeCamera == camera1 && Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(SwitchFrom1To2());
        }
        else if (activeCamera == camera2 && Input.GetKeyDown(KeyCode.S))
        {
            SwitchFrom2To1();
        }
    }

    IEnumerator SwitchFrom1To2()
    {
        isSwitching = true;

        yield return new WaitForSeconds(3f);

        SetCameraState(camera1, false);
        SetCameraState(camera2, true);
        activeCamera = camera2;

        // 2번 카메라에서 UI 활성화
        UpdateCanvasRaycast();

        isSwitching = false;
    }

    void SwitchFrom2To1()
    {
        SetCameraState(camera2, false);
        SetCameraState(camera1, true);
        activeCamera = camera1;

        // 1번 카메라에서 UI 비활성화
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

    // Canvas 클릭 가능/불가능 제어
    private void UpdateCanvasRaycast()
    {
        if (targetCanvas != null)
        {
            GraphicRaycaster gr = targetCanvas.GetComponent<GraphicRaycaster>();
            if (gr != null)
                gr.enabled = (activeCamera == camera2); // 2번 카메라만 클릭 가능
        }
    }
}
