using Project.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Project
{
    [RequireComponent(typeof(Camera))]
    public class HubCameraController : MonoBehaviour
    {
        public Camera Camera
        {
            get;
            private set;
        }

        void Start()
        {
            Camera = GetComponent<Camera>();
            var cameraData = Camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(UISystem.Instance.Camera);
        }
    }
}