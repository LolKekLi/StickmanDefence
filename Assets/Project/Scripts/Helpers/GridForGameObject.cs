#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project
{
	[ExecuteAlways]
	public class GridForGameObject : MonoBehaviour
	{
		[SerializeField]
		private GridSettings _gridSettings = null;
        
		[field : SerializeField]
		public Transform Center
		{
			get;
			private set;
		}
		
        [SerializeField]
		private Transform[] _elements = null;

		private void Update()
		{
			var contains = Selection.Contains(gameObject);
			if (Application.isPlaying || _gridSettings == null)
			{
				return;
			}

			_elements = GetChild();

			GridElement();
		}

		private Transform[] GetChild()
		{
			var _child = new List<Transform>();

			var componentsInChildren = transform.GetComponentsInChildren<Transform>();

			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].parent == transform)
				{
					_child.Add(componentsInChildren[i]);
				}
			}

			return _child.ToArray();
		}

		private void GridElement()
		{
			if (_elements == null || Center == null)
			{
				return;
			}

			var halfGridSize = _gridSettings.GridSize / 2;
			var centerPos = Center.position;
			var startElementPos = centerPos + Center.TransformDirection(new Vector3( - halfGridSize.x + _gridSettings.ElementBoarderSize / 2, 0,  halfGridSize.y)) ;
			var rightBorderX = centerPos.x + halfGridSize.x;

			var size = new Vector3[10, 10];

			for (var i = 0; i < size.GetUpperBound(0) + 1; i++)
			{
				for (int j = 0; j < size.GetUpperBound(1) + 1; j++)
				{
					size[i, j] =  startElementPos  + Center.TransformDirection( new Vector3((i * _gridSettings.Spacing),
					                                                                                      _gridSettings.YPos, -(j * _gridSettings.LineSpace)));
				}
			}

			int i1 = 0;

			for (int i = 0, j = 0; i < _elements.Length && i1 < size.GetUpperBound(0) + 1 && j < size.GetUpperBound(1) + 1; i++, i1++)
			{
				if (size[i1, j].x + _gridSettings.ElementBoarderSize / 2 >= rightBorderX)
				{
					j++;
					i1 -= i / j;
				}

				_elements[i].transform.position = size[i1, j];
				//_elements[i].transform.rotation = _center.rotation;
				
				EditorUtility.SetDirty(_elements[i]);
			}
		}

		private void OnDrawGizmos()
		{
			if (Center == null || _gridSettings == null)
			{
				return;
			}
			DrawGridBorder();
			DrawElementsBorder();
		}
		
		private void DrawElementsBorder()
		{
			if (_elements == null)
			{
				return;
			}

			Gizmos.color = Color.red;

			Vector3 borderSize = new Vector3(_gridSettings.ElementBoarderSize, 0, 2);

			for (int i = 0; i < _elements.Length; i++)
			{
				if (_elements[i] == null)
				{
					return;
				}

				var centralPoint = _elements[i].transform.position;
				
				var rotationMatrix = Matrix4x4.TRS(_elements[i].transform.position,_elements[i].transform.rotation, Vector3.one);
				Gizmos.matrix = rotationMatrix;

				Gizmos.DrawWireCube(Vector3.zero, borderSize);
			}
		}

		private void DrawGridBorder()
		{
			Gizmos.color = Color.yellow;
			
			var rotationMatrix = Matrix4x4.TRS(Center.position, Center.rotation, Vector3.one);
			Gizmos.matrix = rotationMatrix;
 
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(_gridSettings.GridSize.x, 0.1f, _gridSettings.GridSize.y));
		}

	}
}
#endif