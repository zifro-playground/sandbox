using System;
using PM;
using UnityEngine;

namespace Zifro.Sandbox.UI
{
	public class SandboxRunCodeButton : RunCodeButton
	{
		[SerializeField, HideInInspector]
		CodeRunner codeRunner;

		void Awake()
		{
			codeRunner = FindObjectOfType<CodeRunner>();
			Debug.Assert(codeRunner, "CodeRunner not found");
		}

		public new void OnRunCodeButtonClick()
		{
			if (codeRunner.isRunning)
			{
				if (codeRunner.isPaused)
				{
					codeRunner.PauseRunning();
				}
				else
				{
					codeRunner.ResumeRunning();
				}
			}
			else
			{
				codeRunner.StartRunning();
			}
		}

		public void OnStopCodeButtonClick()
		{
			codeRunner.StopRunning(StopStatus.UserForced);
		}
	}
}
