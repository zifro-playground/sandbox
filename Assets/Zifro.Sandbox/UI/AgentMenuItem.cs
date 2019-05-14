using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuItem : MonoBehaviour
	{
		public Button button;

		void Awake()
		{
			Debug.Assert(button, $"{nameof(button)} is not assigned for {name}.", this);
		}
	}
}
