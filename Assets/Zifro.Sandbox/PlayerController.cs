using System;
using PM;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class PlayerController : MonoBehaviour, IPMCompilerStarted, IPMCompilerStopped
	{
		public FractionVector3 fractionPosition;
		FractionVector3 startingPosition;

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

		FractionVector3 GetDirectionVectorInt(Direction direction)
		{
			switch (direction)
			{
			case Direction.North: return new FractionVector3(0, 0, 1);
			case Direction.East: return new FractionVector3(-1, 0, 0);
			case Direction.South: return new FractionVector3(0, 0, -1);
			case Direction.West: return new FractionVector3(1, 0, 0);
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public void OnPMCompilerStarted()
		{
			startingPosition = fractionPosition;
		}

		public void OnPMCompilerStopped(StopStatus status)
		{
			fractionPosition = startingPosition;
			transform.position = startingPosition;
		}
	}
}
