using UnityEngine;

namespace Project
{
    public class CantSpawnZone : MonoBehaviour
    {
        [SerializeField]
        private Bounds _bounds = default;

        private bool _isVisualNeeded = true;
        
        public bool Contains(Vector3 transformPosition)
        {
            return _bounds.Contains(transformPosition);
        }
        
        public void ChangeBorderCenter(Vector3 newCenter)
        {
            _bounds.center = newCenter;
        }
        
        public void ToggleVisual(bool isVisualNeeded)
        {
            _isVisualNeeded = isVisualNeeded;
        }

        #region Gizmos
        private void OnDrawGizmos()
        {
            if (_isVisualNeeded)
            {
                DrawBounds(_bounds);    
            }
        }

        void DrawBounds(Bounds b)
        {
            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Gizmos.DrawLine(p5, p6);
            Gizmos.DrawLine(p6, p7);
            Gizmos.DrawLine(p7, p8);
            Gizmos.DrawLine(p8, p5);

            // sides
            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p7);
            Gizmos.DrawLine(p4, p8);
        }
        #endregion
    }
}