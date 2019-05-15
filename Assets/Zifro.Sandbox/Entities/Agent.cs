using System;
using System.Collections.Generic;
using UnityEngine;
using Zifro.Sandbox.UI;

namespace Zifro.Sandbox.Entities
{
	[Serializable]
	public class Agent
	{
		public string name;

		[TextArea]
		public string code;

		[Space]
		public GameObject agentPrefab;
		public GameObject modelPrefab;
		public AgentMenuItem menuItem;

		[Header("Instance tracking")]
		public List<AgentInstance> instances;
	}
}
