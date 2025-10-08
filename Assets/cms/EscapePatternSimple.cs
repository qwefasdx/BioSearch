using UnityEngine;
using System.Collections;

public class EscapePatternSimple : MonoBehaviour
{
    public Transform pointA;      // 시작 위치
    public Transform pointB;      // 목표 위치
    public float speed = 2f;      // 이동 속도
    private bool isMoving = false;

    // A -> B 한 번 이동 (탈주 시 호출)
    public void MoveToEscape()
    {
        StopAllCoroutines(); // 현재 이동 중이면 즉시 멈추고 새로 시작
        StartCoroutine(MoveTo(pointB));
    }

    // B -> A 복귀 (문 클릭 시 호출)
    public void MoveBack()
    {
        StopAllCoroutines();
        StartCoroutine(MoveTo(pointA));
    }

    IEnumerator MoveTo(Transform target)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }
}
