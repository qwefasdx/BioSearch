using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera targetCamera;
    public float defaultFOV = 60f;
    public float zoomFOV = 40f;
    public float transitionSpeed = 5f;

    private float targetFOV;

    public Transform view1;     // 첫 번째 시점 (W)
    public Transform view2;     // 두 번째 시점 (S, 기본 시작점)
    public Transform leftView;  // S 상태일 때 왼쪽 (A)
    public Transform rightView; // S 상태일 때 오른쪽 (D)

    private Transform currentView;
    private bool inView2 = false;   // 현재 S 시점인지
    private bool mustPassThroughS = false; // A,D 누를 때 S를 거쳐야 하는지 체크

    void Start()
    {
        // 시작할 때 S 뷰에서 시작
        currentView = view2;
        transform.position = view2.position;
        transform.rotation = view2.rotation;
        inView2 = true;
        targetFOV = defaultFOV;

        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // W → view1 전환 (조건: 현재 S 시점일 때만 가능)
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (inView2 && currentView == view2) // S 시점일 때만 허용
            {
                currentView = view1;
                inView2 = false;
                targetFOV = zoomFOV;
            }
        }

        // S → view2 전환
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentView != view2) // 이미 S시점이면 무시
            {
                currentView = view2;
                inView2 = true;
                mustPassThroughS = true; // S를 직접 누르면 다시 초기화
                targetFOV = defaultFOV;
            }
        }

        // S 상태일 때만 A, D 작동
        if (inView2)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentView == leftView) return; // 이미 A시점이면 무시

                if (!mustPassThroughS)
                {
                    currentView = view2;   // 먼저 S로 이동
                    mustPassThroughS = true;
                }
                else
                {
                    currentView = leftView;
                    mustPassThroughS = false; // 리셋
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentView == rightView) return; // 이미 D시점이면 무시

                if (!mustPassThroughS)
                {
                    currentView = view2;
                    mustPassThroughS = true;
                }
                else
                {
                    currentView = rightView;
                    mustPassThroughS = false;
                }
            }
        }

        // 카메라 부드럽게 이동/회전
        transform.position = Vector3.Lerp(
            transform.position,
            currentView.position,
            Time.deltaTime * transitionSpeed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            currentView.rotation,
            Time.deltaTime * transitionSpeed
        );

        // FOV 전환
        targetCamera.fieldOfView = Mathf.Lerp(
            targetCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * transitionSpeed
        );
    }
}
