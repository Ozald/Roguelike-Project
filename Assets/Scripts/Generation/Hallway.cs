public class Hallway : Connectable
{
    private Connectable? origin;
    private Connectable? end;

    public Connectable? Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    public Connectable? End
    {
        get { return end; }
        set { end = value; }
    }

    public Hallway()
    {
        Connections = new[] { Origin, End };
    }

    public override string ToString()
    {
        return "H";
    }
}