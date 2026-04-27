using UnityEngine;

public class RouteSelector : MonoBehaviour
{
    public ServiceQueue ComputeQueue;
    public ServiceQueue SimpleQueue;
    public AgentSink RejectSink;

    public void Route(Agent a)
    {
        if (a == null) return;
        ServiceQueue target = (a.Task == Agent.TaskType.Compute)
            ? ComputeQueue : SimpleQueue;

        if (target == null)
        {
            Debug.LogError($"RouteSelector: target queue not assigned for task {a.Task}");
            Destroy(a.gameObject);
            return;
        }

        if (!target.TryEnqueue(a))
        {
            StatsCollector.I?.OnRejected();
            if (RejectSink != null) RejectSink.Accept(a);
            else Destroy(a.gameObject);
        }
    }
}