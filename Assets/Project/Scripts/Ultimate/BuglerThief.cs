using Project;
using UnityEngine;

public class BuglerThief : Ultimate
{
    [SerializeField]
    private ParticleSystem _particle;
    
    public override UltimateType UltimateType
    {
        get => UltimateType.BuglerThief;
    }

    public override void Apply()
    {
        _particle.Play();
    }
}