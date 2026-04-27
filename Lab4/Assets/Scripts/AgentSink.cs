using UnityEngine;

public class AgentSink : MonoBehaviour
{
    public Transform ExitPoint;
    public float DespawnDelay = 2f;

    public virtual void Accept(Agent a)
    {
        if (a == null) return;
        if (ExitPoint != null) a.GoTo(ExitPoint.position);
        Destroy(a.gameObject, DespawnDelay);
    }
}