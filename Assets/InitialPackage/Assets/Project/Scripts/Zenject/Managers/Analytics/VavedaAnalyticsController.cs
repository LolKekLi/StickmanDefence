using UnityEngine;

#if UNITY_AMAZON
using FAdsSdk;
using Vaveda.Integration.Scripts;
using Vaveda.Integration.Scripts.Analytics;
using Vaveda.Integration.Scripts.Analytics.Builders;
#endif

namespace Project
{
    public class VavedaAnalyticsController : AnalyticsController
    {
        private float _startLevelTime = 0f;
        private float _endLevelTime = 0f;
        
        private uint GetLevelCompleteTime
        {
            get => (uint)(_endLevelTime - _startLevelTime);
        }
        
        public override void Init(LevelSettings levelSettings)
        {
            base.Init(levelSettings);
            
            ServicesInitialize();
        }

        private void ServicesInitialize()
        {
#if UNITY_AMAZON
            var analyticsBuilder = new AnalyticsBuilder()
                .Add<VavedaAnalyticsService>();

            var adMask = AdShowType.Everything ;
            
            Services.Instance.Initialize(analyticsBuilder, adMask);
#endif
        }

        public override void TrackStart()
        {
#if UNITY_AMAZON
            _startLevelTime = Time.time;

            Services.Instance.AnalyticsService.LevelStartEvent(_levelSettings.GetScene,
                (uint)LocalConfig.LevelIndex);
            
            DebugSafe.Log($"level_started. Level Name - {_levelSettings.GetScene}. Level Index - {LocalConfig.LevelIndex}");
#endif
            
        }

        public override void TrackFinish()
        {
#if UNITY_AMAZON
            _endLevelTime = Time.time;

            Services.Instance.AnalyticsService.LevelEndEvent(_levelSettings.GetScene,
                (uint)LocalConfig.LevelIndex,
                (uint)GetLevelCompleteTime, 1);
            
            DebugSafe.Log(
                $"level_finish. Level Name - {_levelSettings.GetScene}. Level Index - {LocalConfig.LevelIndex}. Level Time - {GetLevelCompleteTime}");
#endif
        }

        public override void TrackFail()
        {
#if UNITY_AMAZON
            _endLevelTime = Time.time;
            
            Services.Instance.AnalyticsService.LevelEndEvent(_levelSettings.GetScene,
                (uint)LocalConfig.LevelIndex,
                GetLevelCompleteTime, 0);
            
            DebugSafe.Log(
                $"level_fail. Level Name - {_levelSettings.GetScene}. Level Index - {LocalConfig.LevelIndex}. Level Time - {GetLevelCompleteTime}");
#endif
        }
    }
}