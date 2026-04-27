using System.Collections;
using UnityEngine;

public class ServiceChannel : MonoBehaviour
{
    [Header("Подключения")]
    public ServiceQueue Source;
    public AgentSink ExitSink;

    [Header("Параметры обслуживания")]
    public Transform[] ServicePoints;
    public float TimeMin = 5f;
    public float TimeMode = 10f;
    public float TimeMax = 20f;
    public float TimeScale = 60f;

    private bool[] busy;

    private void Start()
    {
        if (ServicePoints == null || ServicePoints.Length == 0)
        {
            Debug.LogError($"{name}: ServicePoints не задан!");
            return;
        }
        if (Source == null)
        {
            Debug.LogError($"{name}: Source не задан!");
            return;
        }
        busy = new bool[ServicePoints.Length];
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            int slot = FindFreeSlot();
            if (slot >= 0 && Source.Count > 0)
            {
                Agent a = Source.Dequeue();
                if (a != null) StartCoroutine(Serve(a, slot));
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < busy.Length; i++)
            if (!busy[i]) return i;
        return -1;
    }

    private IEnumerator Serve(Agent a, int slot)
    {
        busy[slot] = true;
        a.GoTo(ServicePoints[slot].position);

        // ждём, пока агент дойдёт до точки обслуживания (с защитой от зависания)
        float waitTimeout = 30f;
        float t0 = Time.time;
        while (!a.HasArrived() && Time.time - t0 < waitTimeout)
            yield return null;

        float duration = Triangular(TimeMin, TimeMode, TimeMax);
        StatsCollector.I?.OnServiceStart(name, duration);
        // умножаем на 60 (минуты → секунды) и делим на TimeScale (ускорение)
        yield return new WaitForSeconds(duration * 60f / TimeScale);
        StatsCollector.I?.OnServiceEnd(name, duration);

        busy[slot] = false;

        if (ExitSink != null) ExitSink.Accept(a);
        else Destroy(a.gameObject);
    }

    public float Utilization()
    {
        if (busy == null) return 0f;
        int b = 0;
        foreach (bool x in busy) if (x) b++;
        return (float)b / busy.Length;
    }

    private float Triangular(float a, float c, float b)
    {
        float u = Random.value;
        float fc = (c - a) / (b - a);
        if (u < fc) return a + Mathf.Sqrt(u * (b - a) * (c - a));
        return b - Mathf.Sqrt((1 - u) * (b - a) * (b - c));
    }
}