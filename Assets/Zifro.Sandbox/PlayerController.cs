using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using PM;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class PlayerController : MonoBehaviour, IPMCompilerStarted, IPMCompilerStopped
	{
		public FractionVector3 fractionPosition;
		FractionVector3 startingFractionPosition;
		Vector3 lastPosition;

		int fractionRotation;
		Quaternion newRotation;
		Quaternion lastRotation;
		int startingFractionRotation;

		float passedTime;

		const int ROTATION_SCALE = 360 / FractionVector3.SCALE;

		void Start()
		{
			Transform self = transform;
			fractionRotation = (int)(self.eulerAngles.y / ROTATION_SCALE);
			newRotation = self.rotation;

			Vector3 position = self.position;
			fractionPosition = new Vector3Int(
				Mathf.RoundToInt(position.x),
				Mathf.RoundToInt(position.y),
				Mathf.RoundToInt(position.z)
			);

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

		public void Walk(Direction direction)
		{
			fractionPosition += GetDirectionFraction(direction) * Time.fixedDeltaTime;
		}

		public void Rotate(Rotation rotation)
		{
			fractionRotation += GetRotationFraction(rotation);
			newRotation = Quaternion.Euler(0, fractionRotation * ROTATION_SCALE, 0);
		}

		int GetRotationFraction(Rotation rotation)
		{
			switch (rotation)
			{
			case Rotation.Right: return 1;
			case Rotation.Left: return -1;
			default:
				throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
			}
		}

		FractionVector3 GetDirectionFraction(Direction direction)
		{
			switch (direction)
			{
			case Direction.North: return new FractionVector3(0, 0, FractionVector3.SCALE);
			case Direction.East: return new FractionVector3(FractionVector3.SCALE, 0, 0);
			case Direction.South: return new FractionVector3(0, 0, -FractionVector3.SCALE);
			case Direction.West: return new FractionVector3(-FractionVector3.SCALE, 0, 0);
			case Direction.Forward: return (FractionVector3) transform.forward;
			case Direction.Backward: return -(FractionVector3)transform.forward;
			case Direction.Left: return -(FractionVector3) transform.right;
			case Direction.Right: return (FractionVector3) transform.right;
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public void OnPMCompilerStarted()
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

		public void OnPMCompilerStopped(StopStatus status)
		{
			ResetPositions();
		}

		private void ResetPositions()
		{
			enabled = false;
			fractionPosition = startingFractionPosition;
			transform.position = startingFractionPosition;

			fractionRotation = startingFractionRotation;
			transform.rotation = Quaternion.Euler(0, fractionRotation * ROTATION_SCALE, 0);
		}
	}
}
