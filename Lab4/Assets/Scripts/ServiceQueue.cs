using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceQueue : MonoBehaviour
{
    [Header("Параметры")]
    public int Capacity = 15;
    public float TimeoutSeconds = 0f;       // 0 = без таймаута
    public AgentSink TimeoutSink;
    public Transform QueueAnchor;

    private readonly Queue<Agent> queue = new Queue<Agent>();
    private readonly Dictionary<Agent, Coroutine> waiters
        = new Dictionary<Agent, Coroutine>();

    public bool TryEnqueue(Agent a)
    {
        if (a == null) return false;
        if (queue.Count >= Capacity) return false;

        queue.Enqueue(a);
        if (QueueAnchor != null) a.GoTo(QueueAnchor.position);
        if (TimeoutSeconds > 0)
            waiters[a] = StartCoroutine(WaitOrTimeout(a));
        StatsCollector.I?.OnEnqueue(name);
        return true;
    }

    public Agent Dequeue()
    {
        if (queue.Count == 0) return null;
        Agent a = queue.Dequeue();
        if (waiters.TryGetValue(a, out var c) && c != null) StopCoroutine(c);
        waiters.Remove(a);
        return a;
    }

    public int Count => queue.Count;

    private IEnumerator WaitOrTimeout(Agent a)
    {
        yield return new WaitForSeconds(TimeoutSeconds);
        if (queue.Contains(a))
        {
            // Удаляем агента из очереди (Queue<T> не поддерживает Remove)
            var rest = new Queue<Agent>();
            while (queue.Count > 0)
            {
                var x = queue.Dequeue();
                if (x != a) rest.Enqueue(x);
            }
            while (rest.Count > 0) queue.Enqueue(rest.Dequeue());

            waiters.Remove(a);
            StatsCollector.I?.OnTimeout();
            if (TimeoutSink != null) TimeoutSink.Accept(a);
            else Destroy(a.gameObject);
        }
    }
}