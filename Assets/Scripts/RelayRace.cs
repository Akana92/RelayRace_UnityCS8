using System.Collections.Generic;
using UnityEngine;

public class RelayRace : MonoBehaviour
{
    [SerializeField]
    private Transform[] runners; // ������ ������� �� ����������
    public float speed = 5f; // �������� ��������
    public float passDistance = 1f; // ��������� �������� ��������
    public Transform baton; // ���������� �������

    private int currentRunnerIndex = 0; // ������ �������� ������
    private List<RunnerState> runnerStates = new List<RunnerState>(); // ������ ��������� �������

    private class RunnerState
    {
        public Transform runner;
        public Vector3 initialPosition;
        public bool isReturning; // ���� �������� �� �����

        public RunnerState(Transform runner, Vector3 initialPosition)
        {
            this.runner = runner;
            this.initialPosition = initialPosition;
            this.isReturning = false;
        }
    }

    void Start()
    {
        // �������������� ������ ��������� �������
        for (int i = 0; i < runners.Length; i++)
        {
            runnerStates.Add(new RunnerState(runners[i], runners[i].position));
        }
    }

    void Update()
    {
        if (runnerStates.Count == 0 || baton == null)
            return;

        // ��������� ��������� ������� ������
        foreach (var runnerState in runnerStates)
        {
            Transform runner = runnerState.runner;

            if (baton.parent == runner)
            {
                // ����� � �������� �������� � ���������� ������
                int nextRunnerIndex = (currentRunnerIndex + 1) % runnerStates.Count;
                RunnerState nextRunnerState = runnerStates[nextRunnerIndex];
                Transform nextRunner = nextRunnerState.runner;

                // ������� � ���������� ������
                Vector3 direction = nextRunner.position - runner.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    runner.rotation = Quaternion.Slerp(runner.rotation, targetRotation, Time.deltaTime * 5f);
                }

                // �������� � ���������� ������
                runner.position = Vector3.MoveTowards(runner.position, nextRunner.position, speed * Time.deltaTime);

                // �������� �������� ��������
                if (Vector3.Distance(runner.position, nextRunner.position) < passDistance)
                {
                    // �������� ������� ���������� ������
                    baton.SetParent(nextRunner);
                    baton.localPosition = new Vector3(0, 1.5f, 0.5f); // ��������� �� �������������

                    // ������������� ���� �������� ��� �������� ������
                    runnerState.isReturning = true;

                    // ��������� ������ �������� ������
                    currentRunnerIndex = nextRunnerIndex;
                }
            }
            else if (runnerState.isReturning)
            {
                // ����� ������������ �� �������� �������
                runner.position = Vector3.MoveTowards(runner.position, runnerState.initialPosition, speed * Time.deltaTime);

                // ������� � ������� �������� �������
                Vector3 direction = runnerState.initialPosition - runner.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    runner.rotation = Quaternion.Slerp(runner.rotation, targetRotation, Time.deltaTime * 5f);
                }

                // ���������, ������ �� ����� �������� �������
                if (Vector3.Distance(runner.position, runnerState.initialPosition) < 0.1f)
                {
                    runnerState.isReturning = false;
                }
            }
            else
            {
                // ����� ��� ������� � �� ������������ � ����� �� �����
                continue;
            }
        }
    }

    // ����� ��� ������������ ����� ����� �������� � ���������
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
