using UnityEngine;


public class EntityController : MonoBehaviour
{
    #region Instance
    private static EntityController instance;
    public static EntityController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<EntityController>();
            return instance;
        }
        protected set
        {
            instance = value;
        }
    }
    void Awake()
    {
        Instance = this;
    }
    #endregion
    
    #region Ressources
    //Inpsektor
    public int StartResources = 100;
     
    private int resources = 0;
    public virtual int Resources
    {
        get
        {
            return resources;
        }
        private set
        {
            resources = value;
        }
    }
    public void AddResources(int amount)
    {
        instance.Resources += amount;
    }
    public bool HasResources(int amount)
    {
        return instance.Resources >= amount;
    }
    public bool TakeResources(int amount)
    {
        if (!HasResources(amount))
            return false;
        instance.Resources -= amount;
        return true;
    }
    #endregion

    protected virtual void Start()
    {
        AddResources(StartResources);
    }
}

