namespace Project
{
    public class AttackControllerFactory
    {
        public BaseAttackController GetAttackController(FireType fireType)
        {
            return fireType switch

            {
                FireType.Single => new SimpleAttackController(),
                _ => null
            };
        }
    }
}