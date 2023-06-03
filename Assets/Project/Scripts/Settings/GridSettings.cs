#if UNITY_EDITOR
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "Scriptable/GridSettings", order = 0)]
    public class GridSettings : ScriptableObject
    {
        [field : SerializeField, Header("Grid")]
        public	Vector2 GridSize
        {
            get;
            private set;
        }

        [field : SerializeField]
        public float Spacing
        {
            get;
            private set;
        }

        [field : SerializeField]
        public float LineSpace
        {
            get;
            private	set;
        }

        [field : SerializeField]
        public float ElementBoarderSize
        {
            get;
            private set;
        }

		

        [field : SerializeField]
        public float YPos
        {
            get;
            private set;
        }
    }
}
#endif