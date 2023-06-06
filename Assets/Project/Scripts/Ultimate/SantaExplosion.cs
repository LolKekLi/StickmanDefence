using Project;
using UnityEngine;

public class SantaExplosion : Ultimate
{
    private readonly int EnableKey = Animator.StringToHash("Enabled");
    
    [SerializeField]
    private ParticleSystem _particle;

    [SerializeField]
    private Animator _animator;

    public override UltimateType UltimateType
    {
        get => UltimateType.SantaBigExplosion;
    }
    
    //Вызываеться с аниматора
    public void GetDamage()
    {
        _particle.Play();
    }
    
    public override void Apply()
    {
        _animator.SetTrigger(EnableKey);
    }
}