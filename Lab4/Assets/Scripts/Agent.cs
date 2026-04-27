using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    public enum TaskType { Compute, Simple }

    public TaskType Task { get; private set; }
    public float ArrivalTime { get; private set; }

    private NavMeshAgent navAgent;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        ArrivalTime = Time.time;
        Task = Random.value < 0.4f ? TaskType.Compute : TaskType.Simple;
    }

    public void GoTo(Vector3 destination)
    {
        if (navAgent != null && navAgent.isOnNavMesh)
            navAgent.SetDestination(destination);
    }

    public bool HasArrived()
    {
        return navAgent != null
            && !navAgent.pathPending
            && navAgent.remainingDistance <= navAgent.stoppingDistance + 0.1f;
    }
}