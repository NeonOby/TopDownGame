
public class Fighter : Worker
{
    private Entity target;
    public Entity Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            TargetChanged();
        }
    }

    protected virtual void TargetChanged()
    {

    }
}

