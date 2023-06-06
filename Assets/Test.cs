using Sirenix.OdinInspector;
using UnityEngine;

public class Test : SerializedMonoBehaviour
{
    private float _timeScale;

    private void Start()
    {
        _timeScale = Time.timeScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = _timeScale * 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = _timeScale;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = _timeScale * 3;
        }
    }
}