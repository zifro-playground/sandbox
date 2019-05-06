using System;
using PM;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class PlayerController : MonoBehaviour, IPMCompilerStarted, IPMCompilerStopped
	{
		const int ROTATION_SCALE = 360 / 60;
		public FractionVector3 fractionPosition;

		int fractionRotation;
		Vector3 lastPosition;
		Quaternion lastRotation;
		Quaternion newRotation;

		float passedTime;
		FractionVector3 startingFractionPosition;
		int startingFractionRotation;

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			if (enabled)
			{
				ResetPositions();
			}

			enabled = true;
			startingFractionPosition = fractionPosition;
			lastPosition = startingFractionPosition;

			startingFractionRotation = fractionRotation;
			lastRotation = Quaternion.Euler(0, startingFractionRotation * ROTATION_SCALE, 0);
		}

		void IPMCompilerStopped.OnPMCompilerStopped(StopStatus status)
		{
			ResetPositions();
		}

		public void Walk(Direction direction, float scale = 1)
		{
			fractionPosition += GetDirectionFraction(direction, scale) * Time.fixedDeltaTime;
		}

		public void Rotate(Rotation rotation, float scale = 1)
		{
			fractionRotation += GetRotationFraction(rotation, scale);
			newRotation = Quaternion.Euler(0, fractionRotation * ROTATION_SCALE, 0);
		}

		public int GetRotationFraction(Rotation rotation, float scale)
		{
			switch (rotation)
			{
			case Rotation.Right: return Mathf.RoundToInt(scale);
			case Rotation.Left: return Mathf.RoundToInt(-scale);
			default:
				throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
			}
		}

		public FractionVector3 GetDirectionFraction(Direction direction, float scale)
		{
			switch (direction)
			{
			case Direction.North: return new FractionVector3(0, 0, scale);
			case Direction.East: return new FractionVector3(scale, 0, 0);
			case Direction.South: return new FractionVector3(0, 0, -scale);
			case Direction.West: return new FractionVector3(-scale, 0, 0);
			case Direction.Forward: return (FractionVector3)(transform.forward * scale);
			case Direction.Backward: return -(FractionVector3)(transform.forward * scale);
			case Direction.Left: return -(FractionVector3)(transform.right * scale);
			case Direction.Right: return (FractionVector3)(transform.right * scale);
			case Direction.Up: return new FractionVector3(0, scale, 0);
			case Direction.Down: return new FractionVector3(0, -scale, 0);
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		void Start()
		{
			Transform self = transform;
			fractionRotation = (int)(self.eulerAngles.y / ROTATION_SCALE);
			newRotation = self.rotation;

			fractionPosition = (FractionVector3)self.position;

			enabled = false;
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			transform.position = fractionPosition;
		}
#endif

		void FixedUpdate()
		{
			passedTime = 0;
			lastPosition = fractionPosition;

			lastRotation = Quaternion.Euler(0, fractionRotation * ROTATION_SCALE, 0);
		}

		void Update()
		{
			passedTime += Time.deltaTime * Time.fixedDeltaTime;
			transform.position = Vector3.Lerp(lastPosition, fractionPosition, passedTime);
			transform.rotation = Quaternion.Lerp(lastRotation, newRotation, passedTime);
		}

		void ResetPositions()
		{
			enabled = false;
			fractionPosition = startingFractionPosition;
			transform.position = startingFractionPosition;

			fractionRotation = startingFractionRotation;
			transform.rotation = Quaternion.Euler(0, fractionRotation * ROTATION_SCALE, 0);
		}
	}
}
