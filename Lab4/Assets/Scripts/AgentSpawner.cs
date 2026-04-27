using System.Collections;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [Header("Префаб и точки")]
    public GameObject AgentPrefab;
    public Transform SpawnPoint;
    public ServiceQueue FirstQueue;

    [Header("Параметры")]
    public float Lambda = 0.5f;
    public float TimeScale = 60f;

    private void Start()
    {
        if (AgentPrefab == null || FirstQueue == null || SpawnPoint == null)
        {
            Debug.LogError("Spawner: не заданы AgentPrefab, FirstQueue или SpawnPoint");
            return;
        }
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = -Mathf.Log(1f - Random.value) / Lambda;
            yield return new WaitForSeconds(interval * 60f / TimeScale);

            GameObject go = Instantiate(AgentPrefab,
                                        SpawnPoint.position,
                                        Quaternion.identity);
            Agent a = go.GetComponent<Agent>();
            if (!FirstQueue.TryEnqueue(a))
            {
                StatsCollector.I?.OnRejected();
                Destroy(go);
            }
            else
            {
                StatsCollector.I?.OnSpawn();
            }
        }
    }
}