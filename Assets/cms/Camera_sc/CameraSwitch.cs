using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera targetCamera;
    public float defaultFOV = 60f;
    public float zoomFOV = 40f;
    public float transitionSpeed = 5f;

    private float targetFOV;

    public Transform view1;     // ù ��° ���� (W)
    public Transform view2;     // �� ��° ���� (S, �⺻ ������)
    public Transform leftView;  // S ������ �� ���� (A)
    public Transform rightView; // S ������ �� ������ (D)

    private Transform currentView;
    private bool inView2 = false;   // ���� S ��������
    private bool mustPassThroughS = false; // A,D ���� �� S�� ���ľ� �ϴ��� üũ

    void Start()
    {
        // ������ �� S �信�� ����
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
        // W �� view1 ��ȯ (����: ���� S ������ ���� ����)
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (inView2 && currentView == view2) // S ������ ���� ���
            {
                currentView = view1;
                inView2 = false;
                targetFOV = zoomFOV;
            }
        }

        // S �� view2 ��ȯ
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentView != view2) // �̹� S�����̸� ����
            {
                currentView = view2;
                inView2 = true;
                targetFOV = defaultFOV;
            }
        }

        // S ������ ���� A, D �۵�
        if (inView2)
        {
            if (Input.GetKeyDown(KeyCode.A) && currentView != leftView)
            {
                currentView = leftView; // �ٷ� ���� �������� �̵�
            }

            if (Input.GetKeyDown(KeyCode.D) && currentView != rightView)
            {
                currentView = rightView; // �ٷ� ������ �������� �̵�
            }
        }

        // ī�޶� �ε巴�� �̵�/ȸ��
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

        // FOV ��ȯ
        targetCamera.fieldOfView = Mathf.Lerp(
            targetCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * transitionSpeed
        );
    }

}
