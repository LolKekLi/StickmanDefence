#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
	[ExecuteAlways]
	public class GridForGameObject : MonoBehaviour
	{
		[SerializeField, Header("Grid")]
		private	Vector2 _gridSize = Vector2.zero;
		[SerializeField]
		private float _spacing = 0;
		[SerializeField]
		private float _lineSpace = 0;
		[SerializeField]
		private float _elemntBoarderSize = 0;

		private Transform[] _elements = null; 

		private void Update()
		{
			if (Application.isPlaying)
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
			if (_elements == null)
			{
				return;
			}

			var halfGridSize = _gridSize / 2;
			var centerPos = transform.position;
			var startElementPos = new Vector3(centerPos.x - halfGridSize.x + _elemntBoarderSize / 2, centerPos.y, centerPos.z + halfGridSize.y);
			var rightBorderX = centerPos.x + halfGridSize.x;

			var size = new Vector3[10, 10];

			for (var i = 0; i < size.GetUpperBound(0) + 1; i++)
			{
				for (int j = 0; j < size.GetUpperBound(1) + 1; j++)
				{
					size[i, j] = new Vector3(startElementPos.x + (i * _spacing),
					                         startElementPos.y, startElementPos.z - (j * _lineSpace));
				}
			}

			int i1 = 0;

			for (int i = 0, j = 0; i < _elements.Length && i1 < size.GetUpperBound(0) + 1 && j < size.GetUpperBound(1) + 1; i++, i1++)
			{
				if (size[i1, j].x + _elemntBoarderSize / 2 >= rightBorderX)
				{
					j++;
					i1 -= i / j;
				}

				_elements[i].transform.position = size[i1, j];
			}

		}

		private void OnDrawGizmos()
		{
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

			Vector3 borderSize = new Vector3(_elemntBoarderSize, 0, 2);

			for (int i = 0; i < _elements.Length; i++)
			{
				if (_elements[i] == null)
				{
					return;
				}
				
				var centralPoint = _elements[i].transform.position;
				
				Gizmos.DrawWireCube(centralPoint.ChangeY(centralPoint.y + 1), borderSize);
			}
		}

		private void DrawGridBorder()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x, 0, _gridSize.y));
		}

	}
}
#endif