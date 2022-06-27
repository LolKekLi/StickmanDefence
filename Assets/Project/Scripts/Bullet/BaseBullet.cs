using Project;

public class BaseBullet : Bullet
{
    public override DamageType DamageType
    {
        get => DamageType.Base;
    }
}
