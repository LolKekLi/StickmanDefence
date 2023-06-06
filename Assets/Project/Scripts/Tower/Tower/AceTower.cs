using System;

namespace Project
{
    public abstract class AceTower : BaseTower
    {
        public static event Action<UltimateType> Built = delegate {  };
        public static event Action<UltimateType> Selled = delegate {  };
        public override bool IsAssTower
        {
            get => true;
        }

        public abstract UltimateType UltimateType
        {
            get;
        }

        public override void OnBuild(BaseAttackController attackController)
        {
            base.OnBuild(attackController);

            Built(UltimateType);
        }

        public override void Sell()
        {
            base.Sell();

            Selled(UltimateType);
        }
    }
}