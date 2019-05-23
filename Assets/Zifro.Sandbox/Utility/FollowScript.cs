using System;
using UnityEngine;

namespace Zifro.Sandbox.Utility
{
	public class FollowScript : MonoBehaviour
	{
		public Transform target;
		public Vector3Bool followPosition = Vector3Bool.allTrue;
		public Vector3Bool followRotation = Vector3Bool.allFalse;

		void LateUpdate()
		{
			Transform self = transform;

			if (followPosition.all)
			{
				self.position = target.position;
			}
			else if (followPosition.any)
			{
				Vector3 position = self.position;
				Vector3 targetPosition = target.position;
				if (followPosition.x)
				{
					position.x = targetPosition.x;
				}

				if (followPosition.y)
				{
					position.y = targetPosition.y;
				}

				if (followPosition.z)
				{
					position.z = targetPosition.z;
				}

				self.position = position;
			}


			if (followRotation.all)
			{
				self.eulerAngles = target.eulerAngles;
			}
			else if (followRotation.any)
			{
				Vector3 rotation = self.eulerAngles;
				Vector3 targetRotation = target.eulerAngles;

				if (followRotation.x)
				{
					rotation.x = targetRotation.x;
				}

				if (followRotation.y)
				{
					rotation.y = targetRotation.y;
				}

				if (followRotation.z)
				{
					rotation.z = targetRotation.z;
				}

				self.eulerAngles = rotation;
			}
		}

		[Serializable]
		public struct Vector3Bool
		{
			public bool x;
			public bool y;
			public bool z;

			public bool any => x || y || z;
			public bool all => x && y && z;

			public static Vector3Bool allTrue { get; } = new Vector3Bool(true, true, true);
			public static Vector3Bool allFalse { get; } = new Vector3Bool(false, false, false);

			public Vector3Bool(bool x, bool y, bool z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}
	}
}
