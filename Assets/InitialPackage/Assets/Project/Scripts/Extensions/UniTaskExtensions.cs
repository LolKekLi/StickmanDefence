using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public static class UniTaskExtensions
    {
        public static async UniTask Lerp(Action<float> action, float executionTime, AnimationCurve curve,
            CancellationToken token)
        {
            float time = 0f;
            float progress = 0f;

            while (time < executionTime)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                time += Time.deltaTime;
                progress = time / executionTime;
                progress = curve.Evaluate(progress);

                action(progress);
            }
        }
    }
}