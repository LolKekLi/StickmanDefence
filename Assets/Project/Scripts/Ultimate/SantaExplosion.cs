using System;
using Project;
using UnityEngine;

public class SantaExplosion : Ultimate
{
    private readonly int EnableKey = Animator.StringToHash("Enabled");
    
    [SerializeField]
    private ParticleSystem _particle;

    [SerializeField]
    private Animator _animator;

    private Action _unknown;

    public override UltimateType UltimateType
    {
        get => UltimateType.SantaBigExplosion;
    }
    
    //Вызываеться с аниматора
    public void GetDamage()
    {
        _particle.Play();
        
        _unknown.Invoke();
    }
    
    public override void Apply(Action unknown)
    {
        _unknown = unknown;
        _animator.SetTrigger(EnableKey);
    }
}