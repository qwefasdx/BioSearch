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

    //  View 상태
    private enum ViewMode { Front, Left, Right }
    private ViewMode currentView = ViewMode.Front;

    //  W 타이머 관련
    private float wTimer = 0f;
    private bool wTimerActive = false;
    public float switchDelay = 1.5f; // 전환 대기 시간

    void Start()
    {
        SetCameraState(camera1, true);
        SetCameraState(camera2, false);
        activeCamera = camera1;
        UpdateCanvasRaycast();
        currentView = ViewMode.Front; // 기본 시작 상태
    }

    void Update()
    {
        if (isSwitching) return;
        if (targetInputField != null && targetInputField.isFocused) return;

        //  camera1 + Front 상태일 때만 W 전환 가능
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
                    wTimerActive = false; // 타이머 취소
                    return;
                }

                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S))
                {
                    return; // 다른 키 누르면 일시정지
                }

                wTimer -= Time.deltaTime;

                if (wTimer <= 0f)
                {
                    StartCoroutine(SwitchFrom1To2());
                    wTimerActive = false;
                }
            }
        }

        //  camera2 상태에서 S로 복귀
        if (activeCamera == camera2 && Input.GetKeyDown(KeyCode.S))
        {
            SwitchFrom2To1();
        }

        //  A, D로 시점 전환 (camera1 상태에서만)
        if (activeCamera == camera1)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentView = ViewMode.Left;
                // 카메라 이동/회전 로직 추가
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                currentView = ViewMode.Right;
                // 카메라 이동/회전 로직 추가
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                currentView = ViewMode.Front; // 정면 복귀
                // 카메라 이동/회전 복귀 로직 추가
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
        wTimerActive = false; // W 타이머 취소
        SetCameraState(camera2, false);
        SetCameraState(camera1, true);
        activeCamera = camera1;
        currentView = ViewMode.Front; // 복귀 시 정면
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
