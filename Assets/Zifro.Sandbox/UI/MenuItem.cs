using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI
{
	public class MenuItem : MonoBehaviour
	{
		public Button button;

		protected void Awake()
		{
			Debug.Assert(button, $"{nameof(button)} is not assigned for {name}.", this);
		}
	}
}
