using System;
using Cysharp.Threading.Tasks;
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

    public async override void Apply(Action unknown)
    {
        _particle.Play();

        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        
        unknown.Invoke();
    }
}