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

		public List<AgentInstance> instances;

		public AgentMenuItem menuItem;
	}
}
