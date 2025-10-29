using UnityEngine;
using System.Collections;

public class EscapePatternSimple : MonoBehaviour
{
    public Transform pointA;      // ���� ��ġ
    public Transform pointB;      // �߰� ��ġ
    public Transform pointC;      // ���� ��ġ
    public float speed = 2f;      // �̵� �ӵ�
    private bool isMoving = false;

    // A -> B -> C �̵� (Ż�� �� ȣ��)
    public void MoveToEscape()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToEscapeSequence());
    }

    // C -> B -> A ���� (�� Ŭ�� �� ȣ��)
    public void MoveBack()
    {
        StopAllCoroutines();
        StartCoroutine(MoveBackSequence());
    }

    private IEnumerator MoveToEscapeSequence()
    {
        isMoving = true;
        yield return MoveTo(pointB);
        yield return MoveTo(pointC);
        isMoving = false;
        Debug.Log("[EscapePatternSimple] �� ���տ� ���� ");

    }

    private IEnumerator MoveBackSequence()
    {
        isMoving = true;
        yield return MoveTo(pointB);
        yield return MoveTo(pointA);
        isMoving = false;
        Debug.Log("[EscapePatternSimple] �� �� ��ġ�� ���ư� ");
    }

    private IEnumerator MoveTo(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
