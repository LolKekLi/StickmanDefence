using Project.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Project
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera _camera = null;

        void Start()
        {
            _camera = GetComponent<Camera>();
            var cameraData = _camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(UISystem.Instance.Camera);

        }

    }
}
