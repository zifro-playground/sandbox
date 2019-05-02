using System;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class PlayerController : MonoBehaviour
	{
		public FractionVector3 fractionPosition;

		void Start()
		{
			Vector3 position = transform.position;

			fractionPosition = new Vector3Int(
				Mathf.RoundToInt(position.x),
				Mathf.RoundToInt(position.y),
				Mathf.RoundToInt(position.z)
			);
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			transform.position = fractionPosition;
		}
#endif

		public void Walk(Direction direction)
		{
			fractionPosition += GetDirectionVectorInt(direction);

			transform.position = fractionPosition;
		}

		Vector3Int GetDirectionVectorInt(Direction direction)
		{
			switch (direction)
			{
			case Direction.North: return new Vector3Int(0, 0, 1);
			case Direction.East: return new Vector3Int(-1, 0, 0);
			case Direction.South: return new Vector3Int(0, 0, -1);
			case Direction.West: return new Vector3Int(1, 0, 0);
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}
	}
}
