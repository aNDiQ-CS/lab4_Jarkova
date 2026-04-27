using UnityEngine;
using TMPro;

public class StatsCollector : MonoBehaviour
{
    public static StatsCollector I { get; private set; }

    [Header("UI")]
    public TMP_Text TextSpawned;
    public TMP_Text TextServed;
    public TMP_Text TextTimeout;
    public TMP_Text TextRejected;
    public TMP_Text TextUtilization;

    [Header("Channels (для % загрузки)")]
    public ServiceChannel ChannelCalc;
    public ServiceChannel ChannelSimple;

    private int spawned, served, timeoutCnt, rejected;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public void OnSpawn() => spawned++;
    public void OnRejected() => rejected++;
    public void OnTimeout() => timeoutCnt++;
    public void OnEnqueue(string q) { }
    public void OnServiceStart(string ch, float t) { }
    public void OnServiceEnd(string ch, float t) => served++;

    private void Update()
    {
        if (TextSpawned) TextSpawned.text = $"Создано: {spawned}";
        if (TextServed) TextServed.text = $"Обслужено: {served}";
        if (TextTimeout) TextTimeout.text = $"Отказов: {timeoutCnt}";
        if (TextRejected) TextRejected.text = $"Переполнение: {rejected}";
        if (TextUtilization && ChannelCalc && ChannelSimple)
            TextUtilization.text =
                $"Загрузка calc: {ChannelCalc.Utilization() * 100:0}% | " +
                $"simple: {ChannelSimple.Utilization() * 100:0}%";
    }
}