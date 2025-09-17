using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // 스크립트 상단에 변수 추가
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
        // W → view1 전환
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentView = view1;
            inView2 = false;
            targetFOV = zoomFOV;
        }

        // S → view2 전환
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentView = view2;
            inView2 = true;
            mustPassThroughS = true; // S를 직접 누르면 다시 초기화
            targetFOV = defaultFOV;
        }


        // S 상태일 때만 A, D 작동
        if (inView2)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (!mustPassThroughS) // 아직 S를 거치지 않았다면
                {
                    currentView = view2;   // 먼저 S로 이동
                    mustPassThroughS = true;
                }
                else // 이미 S를 거쳤다면
                {
                    currentView = leftView;
                    mustPassThroughS = false; // 리셋
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (!mustPassThroughS)
                {
                    currentView = view2;   // 먼저 S로 이동
                    mustPassThroughS = true;
                }
                else
                {
                    currentView = rightView;
                    mustPassThroughS = false; // 리셋
                }
            }
        }

        // 카메라 부드럽게 이동/회전
        transform.position = Vector3.Lerp(transform.position, currentView.position, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentView.rotation, Time.deltaTime * transitionSpeed);

        targetCamera.fieldOfView = Mathf.Lerp(
        targetCamera.fieldOfView,
        targetFOV,
        Time.deltaTime * transitionSpeed);
    }
}
