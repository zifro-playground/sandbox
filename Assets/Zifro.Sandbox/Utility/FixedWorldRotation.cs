using UnityEngine;

namespace Zifro.Sandbox.Utility
{
	public class FixedWorldRotation : MonoBehaviour
	{
		public Vector3 eulerAngles;

		void Reset()
		{
			eulerAngles = transform.eulerAngles;
		}

		void OnValidate()
		{
			transform.eulerAngles = eulerAngles;
		}

		void Update()
		{
			transform.eulerAngles = eulerAngles;
		}
	}
}
