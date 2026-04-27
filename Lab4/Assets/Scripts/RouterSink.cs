using UnityEngine;

public class RouterSink : AgentSink
{
    public RouteSelector Router;

    public override void Accept(Agent a)
    {
        if (a == null) return;
        if (Router != null) Router.Route(a);
        else Destroy(a.gameObject);
    }
}