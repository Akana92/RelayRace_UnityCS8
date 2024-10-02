using System.Collections.Generic;
using UnityEngine;

public class RelayRace : MonoBehaviour
{
    [SerializeField]
    private Transform[] runners; // Массив бегунов из инспектора
    public float speed = 5f; // Скорость движения
    public float passDistance = 1f; // Дистанция передачи эстафеты
    public Transform baton; // Эстафетная палочка

    private int currentRunnerIndex = 0; // Индекс текущего бегуна
    private List<RunnerState> runnerStates = new List<RunnerState>(); // Список состояний бегунов

    private class RunnerState
    {
        public Transform runner;
        public Vector3 initialPosition;
        public bool isReturning; // Флаг возврата на место

        public RunnerState(Transform runner, Vector3 initialPosition)
        {
            this.runner = runner;
            this.initialPosition = initialPosition;
            this.isReturning = false;
        }
    }

    void Start()
    {
        // Инициализируем список состояний бегунов
        for (int i = 0; i < runners.Length; i++)
        {
            runnerStates.Add(new RunnerState(runners[i], runners[i].position));
        }
    }

    void Update()
    {
        if (runnerStates.Count == 0 || baton == null)
            return;

        // Обновляем состояние каждого бегуна
        foreach (var runnerState in runnerStates)
        {
            Transform runner = runnerState.runner;

            if (baton.parent == runner)
            {
                // Бегун с палочкой движется к следующему бегуну
                int nextRunnerIndex = (currentRunnerIndex + 1) % runnerStates.Count;
                RunnerState nextRunnerState = runnerStates[nextRunnerIndex];
                Transform nextRunner = nextRunnerState.runner;

                // Поворот к следующему бегуну
                Vector3 direction = nextRunner.position - runner.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    runner.rotation = Quaternion.Slerp(runner.rotation, targetRotation, Time.deltaTime * 5f);
                }

                // Движение к следующему бегуну
                runner.position = Vector3.MoveTowards(runner.position, nextRunner.position, speed * Time.deltaTime);

                // Проверка передачи эстафеты
                if (Vector3.Distance(runner.position, nextRunner.position) < passDistance)
                {
                    // Передача палочки следующему бегуну
                    baton.SetParent(nextRunner);
                    baton.localPosition = new Vector3(0, 1.5f, 0.5f); // Настройте по необходимости

                    // Устанавливаем флаг возврата для текущего бегуна
                    runnerState.isReturning = true;

                    // Обновляем индекс текущего бегуна
                    currentRunnerIndex = nextRunnerIndex;
                }
            }
            else if (runnerState.isReturning)
            {
                // Бегун возвращается на исходную позицию
                runner.position = Vector3.MoveTowards(runner.position, runnerState.initialPosition, speed * Time.deltaTime);

                // Поворот в сторону исходной позиции
                Vector3 direction = runnerState.initialPosition - runner.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    runner.rotation = Quaternion.Slerp(runner.rotation, targetRotation, Time.deltaTime * 5f);
                }

                // Проверяем, достиг ли бегун исходной позиции
                if (Vector3.Distance(runner.position, runnerState.initialPosition) < 0.1f)
                {
                    runnerState.isReturning = false;
                }
            }
            else
            {
                // Бегун без палочки и не возвращается — стоит на месте
                continue;
            }
        }
    }

    // Метод для визуализации путей между бегунами в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (runners != null && runners.Length > 1)
        {
            for (int i = 0; i < runners.Length; i++)
            {
                Transform currentRunner = runners[i];
                Transform nextRunner = runners[(i + 1) % runners.Length];
                if (currentRunner != null && nextRunner != null)
                {
                    Gizmos.DrawLine(currentRunner.position, nextRunner.position);
                }
            }
        }
    }
}
