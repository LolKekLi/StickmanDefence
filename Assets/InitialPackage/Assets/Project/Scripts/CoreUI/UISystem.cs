using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Project.UI
{
    public class UISystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject _windowsContainer = null;

        [SerializeField]
        private Camera _camera = null;

        private Window[] _windows = null;

        private Window _current = null;
        private Stack<Window> _stack = new Stack<Window>();

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        private static UISystem _instance = null;

        [InjectOptional]
        public TowerController TowerController
        {
            get;
            private set;
        }

        public Window CurrentWindow
        {
            get => _current;
        }

        public Camera Camera
        {
            get => _camera;
        }

        public static UISystem Instance
        {
            get => _instance;
        }

        public static Dictionary<string, object> Data
        {
            get => _instance._data;
        }

        private void Awake()
        {
            _instance = this;

            var cameraData = _camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = CameraRenderType.Overlay;


            
            _windows = _windowsContainer.GetComponentsInChildren<Window>(true);
            _windows.Do(wind => wind.Preload());

            DontDestroyOnLoad(gameObject);
        }

        public static void ShowWindow<T>(Dictionary<string, object> data = null)
            where T : Window
        {
            _instance.SetData(data);
            
            var window = GetWindow<T>();
            
            if (!window.IsPopup)
            {
                foreach (var wnd in Instance._stack)
                {
                    wnd.Hide();
                }
                
                Instance._stack.Clear();
            }
            
            Instance._stack.Push(window);

            window.Show(() =>
            {
                if ( Instance._stack.Count > 1)
                {
                    Instance._stack.Pop();
                    Instance._current = Instance._stack.Peek();
                }
            });

            Instance._current = window;
        }

        private static Window GetWindow<T>()
            where T : Window
        {
            var window = Instance._windows.FirstOrDefault(w => w is T);

            if (!window)
            {
                Debug.LogException(new Exception($"{typeof(T)} not found!"));
            }

            return window;
        }

        private void SetData(Dictionary<string, object> data)
        {
            if (data != null)
            {
                foreach (var record in data)
                {
                    if (_data.ContainsKey(record.Key))
                    {
                        _data[record.Key] = record.Value;
                    }
                    else
                    {
                        _data.Add(record.Key, record.Value);
                    }
                }
            }
        }
    }
}