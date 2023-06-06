namespace Project
{
    public class AttackControllerFactory
    {
        public BaseAttackController GetAttackController(FireType fireType)
        {
            return fireType switch

            {
                FireType.Single => new SimpleAttackController(),
                FireType.Granat => new GranatAttackController(),
                _ => null
            };
        }
    }
}