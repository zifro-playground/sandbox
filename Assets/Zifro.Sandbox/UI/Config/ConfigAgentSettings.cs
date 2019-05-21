using UnityEngine;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI.Config
{
	public sealed class ConfigAgentSettings : ConfigMenuItem
	{
		public Text buttonText;
		public string buttonTextFormat = "Inställningar för '{0}'";

		new void Start()
		{
			base.Start();

			Debug.Assert(buttonText, $"{nameof(buttonText)} is not assigned for {name}.", this);
		}

		public void OnAgentSelected(Agent agent)
		{
			buttonText.text = string.Format(buttonTextFormat, agent.name);
		}
	}
}
