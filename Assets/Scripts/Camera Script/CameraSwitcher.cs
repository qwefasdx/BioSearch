using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;   // 1번 카메라
    public Camera camera2;   // 2번 카메라

    private Camera activeCamera;
    private bool isSwitching = false;

    void Start()
    {
        // 시작 시 1번 카메라 활성화, 2번은 비활성화
        SetCameraState(camera1, true);
        SetCameraState(camera2, false);
        activeCamera = camera1;
    }

    void Update()
    {
        if (isSwitching) return;

        // 1번 카메라에서 W → 3초 후 전환
        if (activeCamera == camera1 && Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(SwitchFrom1To2());
        }
        // 2번 카메라에서 S → 즉시 전환
        else if (activeCamera == camera2 && Input.GetKeyDown(KeyCode.S))
        {
            SwitchFrom2To1();
        }
    }

    IEnumerator SwitchFrom1To2()
    {
        isSwitching = true;

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 카메라 전환
        SetCameraState(camera1, false);
        SetCameraState(camera2, true);
        activeCamera = camera2;

        isSwitching = false;
    }

    void SwitchFrom2To1()
    {
        // 즉시 전환
        SetCameraState(camera2, false);
        SetCameraState(camera1, true);
        activeCamera = camera1;
    }

    // 카메라와 오디오 리스너 상태를 함께 관리
    private void SetCameraState(Camera cam, bool state)
    {
        if (cam != null)
        {
            cam.enabled = state;
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null)
            {
                listener.enabled = state;
            }
        }
    }
}
