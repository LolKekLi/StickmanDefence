using PathCreation;
using UnityEngine;

namespace Project
{
    [ExecuteAlways]
    public class PathVisualHelper : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _lineRenderer = null;

        [SerializeField]
        private PathCreator _pathCreator = null;

        private void Start()
        {
            if (Application.isPlaying)
            {
                DrawPathLine();
                Destroy(this);
            }
        }

        private void Update()
        {
            if (_lineRenderer != null)
            {
                DrawPathLine();
            }
        }

        private void DrawPathLine()
        {
            var lineRendererPositionCount = _pathCreator.path.NumPoints;
            _lineRenderer.positionCount = lineRendererPositionCount;

            for (int i = 0; i < lineRendererPositionCount; i++)
            {
                _lineRenderer.SetPosition(i, _pathCreator.path.GetPoint(i));
            }
        }
    }
}