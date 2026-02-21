using UnityEngine;

public abstract class Connectable : MonoBehaviour
{
    protected Connectable?[]? connections;

    public Connectable?[]? Connections
    {
        get { return connections; }
        protected set { connections = value; }
    }
}
