using UnityEngine;

public class PointToPointMover : MonoBehaviour
{
    public Vector3[] points; // ������ ����� ��� ��������
    public float speed = 5f; // �������� ��������

    private int currentPointIndex = 0; // ������� ������ �����
    private bool forward = true; // ����������� ��������: true - �����, false - �����

    void Update()
    {
        if (points.Length == 0)
            return;

        // �������� � ������� �����
        transform.position = Vector3.MoveTowards(transform.position, points[currentPointIndex], speed * Time.deltaTime);

        // ���������� ��������� �� ������� �����
        float distance = Vector3.Distance(transform.position, points[currentPointIndex]);

        // �������� ���������� �����
        if (distance < 0.1f)
        {
            if (forward)
            {
                currentPointIndex++; // �������������� ������
                if (currentPointIndex >= points.Length)
                {
                    currentPointIndex = points.Length - 1;
                    forward = false; // ������ �����������
                }
            }
            else
            {
                currentPointIndex--; // �������������� ������
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 0;
                    forward = true; // ������ �����������
                }
            }
        }
    }

    // ������������ ����� � ����� � ���������
    void OnDrawGizmos()
    {
        if (points == null || points.Length == 0)
            return;

        // ������ �����
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
        }

        // ������ ����� ����� �������
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}
