using UnityEngine;

namespace Zifro.Sandbox.UI
{

	public class AgentMenuItem : MenuItem
	{
		public GameObject modelPrefab;
		public GameObject agentPrefab;

		new void Awake()
		{
			base.Awake();
			Debug.Assert(modelPrefab, $"{nameof(modelPrefab)} is not assigned for {name}.", this);
			Debug.Assert(agentPrefab, $"{nameof(agentPrefab)} is not assigned for {name}.", this);
		}
	}
}
