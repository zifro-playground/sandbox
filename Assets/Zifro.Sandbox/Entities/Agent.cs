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
		public int maxInstanceCount = -1;

		[TextArea]
		public string code;

		[Space]
		public GameObject modelPrefab;
		public AgentMenuItem menuItem;

		[Header("Instance tracking")]
		public List<AgentInstance> instances;
	}
}
