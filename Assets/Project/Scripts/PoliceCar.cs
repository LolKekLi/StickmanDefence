using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project;
using UnityEngine;
using UniTaskExtensions = Project.UniTaskExtensions;

public class PoliceCar : MonoBehaviour
{
    [SerializeField]
    private Light _light1;

    [SerializeField]
    private Light _light2;

    [SerializeField]
    private float _changeTime;

    [SerializeField]
    private AnimationCurve _curve;

    private float _light1StartIntensity;
    private float _light2StartIntensity;

    private CancellationTokenSource _cancellationToken;

    private async void Start()
    {
        _light1StartIntensity = _light1.intensity;
        _light2StartIntensity = _light2.intensity;
        _light2.intensity = 0;

        var cancellationToken = UniTaskUtil.RefreshToken(ref _cancellationToken);

        var targetValue1 = 0f;
        var targetValue2 = _light2StartIntensity;

        while (true)
        {
            SmoothChangeLight(_light1, targetValue1, _changeTime, cancellationToken);
            await SmoothChangeLight(_light2, targetValue2, _changeTime, cancellationToken);

            targetValue1 = targetValue1 == 0 ? _light1StartIntensity : 0;
            targetValue2 = targetValue2 == 0 ? _light2StartIntensity : 0;
        }
    }

    private void OnDisable()
    {
        UniTaskUtil.CancelToken(ref _cancellationToken);
    }

    private async UniTask SmoothChangeLight(Light light, float targetValue, float time, CancellationToken token)
    {
        try
        {
            var startValue = light.intensity;
            await UniTaskExtensions.Lerp(time =>
                {
                    light.intensity = Mathf.Lerp(startValue, targetValue, time);
                }, time,
                _curve, token);
        }
        catch (OperationCanceledException e)
        {
        }
    }
}