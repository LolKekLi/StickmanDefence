namespace Project
{
    public abstract class AnalyticsController
    {
        protected LevelSettings _levelSettings = null;

        public virtual void Init(LevelSettings levelSettings)
        {
            _levelSettings = levelSettings;
        }

        public abstract void TrackStart();
        public abstract void TrackFinish();
        public abstract void TrackFail();
    }
}