using UnityEngine;

public class PointToPointMover : MonoBehaviour
{
    public Vector3[] points; // Массив точек для движения
    public float speed = 5f; // Скорость движения

    private int currentPointIndex = 0; // Текущий индекс точки
    private bool forward = true; // Направление движения: true - вперёд, false - назад

    void Update()
    {
        if (points.Length == 0)
            return;

        // Движение к текущей точке
        transform.position = Vector3.MoveTowards(transform.position, points[currentPointIndex], speed * Time.deltaTime);

        // Вычисление дистанции до текущей точки
        float distance = Vector3.Distance(transform.position, points[currentPointIndex]);

        // Проверка достижения точки
        if (distance < 0.1f)
        {
            if (forward)
            {
                currentPointIndex++; // Инкрементируем индекс
                if (currentPointIndex >= points.Length)
                {
                    currentPointIndex = points.Length - 1;
                    forward = false; // Меняем направление
                }
            }
            else
            {
                currentPointIndex--; // Декрементируем индекс
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 0;
                    forward = true; // Меняем направление
                }
            }
        }
    }

    // Визуализация точек и путей в редакторе
    void OnDrawGizmos()
    {
        if (points == null || points.Length == 0)
            return;

        // Рисуем точки
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
        }

        // Рисуем линии между точками
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}
