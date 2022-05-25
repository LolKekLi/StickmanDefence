namespace Project
{
    public class SayGamesAnalyticsController : AnalyticsController
    {
        private int CurrentLevel
        {
            get => LocalConfig.LevelIndex + 1;
        }
    
        public override void TrackStart()
        {
#if SAYKIT
            SayKit.trackLevelStarted(CurrentLevel);
#endif
        }

        public override void TrackFinish()
        {
#if SAYKIT
            SayKit.trackLevelCompleted(CurrentLevel, 0);
#endif

        }

        public override void TrackFail()
        {
#if SAYKIT
            SayKit.trackLevelFailed(CurrentLevel, 0);
#endif
        }
    }
}