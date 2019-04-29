using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Vector3Int intPosition;

	void Start()
	{
		Vector3 position = transform.position;

		intPosition = new Vector3Int(
			Mathf.RoundToInt(position.x),
			Mathf.RoundToInt(position.y),
			Mathf.RoundToInt(position.z)
		);
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		Vector3 position = transform.position;

		intPosition = new Vector3Int(
			Mathf.RoundToInt(position.x),
			Mathf.RoundToInt(position.y),
			Mathf.RoundToInt(position.z)
		);
	}
#endif

	public void Walk(Direction direction)
	{
		intPosition += GetDirectionVectorInt(direction);

		transform.position = intPosition;
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
