using Project;
using UnityEngine;

public abstract class Ultimate : MonoBehaviour
{
    public abstract UltimateType UltimateType
    {
        get;
    }
    
    public abstract void Apply();
}