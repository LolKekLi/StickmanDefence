using DG.Tweening;

namespace Project.UI
{
    public class SelfTweenController : BaseTweenController
    {
        protected override void Awake()
        {
            base.Awake();

            _animations = GetComponents<DOTweenAnimation>();
        }
    }
}