using System;
using System.Collections.Generic;
using System.Linq;
using Mellis.Core.Entities;
using Mellis.Core.Exceptions;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;
using PM;
using PM.GlobalFunctions;
using UnityEngine;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI;
using Zifro.Sandbox.UI.Config;

namespace Zifro.Sandbox
{
	public class CodeRunner : MonoBehaviour
	{
		static readonly IEmbeddedType[] BUILTIN_FUNCTIONS = {
			new AbsoluteValue(),
			new ConvertToBinary(),
			new ConvertToHexadecimal(),
			new ConvertToOctal(),
			new LengthOf(),
			new RoundedValue(),
			new MinimumValue(),
			new MaximumValue(),
			new GetTime()
		};

		readonly List<AgentProcessor> agentProcessors = new List<AgentProcessor>();

		public bool isRunning { get; private set; }

		public bool isPaused { get; private set; }

		public VariableWindow variableWindow;
		public ConfigMenuList configMenuList;

		AgentBank bank;

		void OnEnable()
		{
			bank = AgentBank.main;
			Debug.Assert(bank, $"Unable to find main agent bank in '{name}'.");
		}

		void Start()
		{
			Debug.Assert(variableWindow, $"{nameof(variableWindow)} not defined in '{name}'.", this);
			Debug.Assert(configMenuList, $"{nameof(configMenuList)} not defined in '{name}'.", this);
			enabled = false;
		}

		void FixedUpdate()
		{
			int ended = 0;
			for (int i = agentProcessors.Count - 1; i >= 0; i--)
			{
				IProcessor processor = agentProcessors[i].processor;
				switch (processor.State)
				{
				case ProcessState.Ended:
				case ProcessState.Error:
					agentProcessors.RemoveAt(i);
					ended++;
					break;

				case ProcessState.Yielded:
					continue;
				}

				try
				{
					WalkStatus result = processor.Walk();

					if (result == WalkStatus.Ended)
					{
						agentProcessors.RemoveAt(i);
						ended++;
					}

					//else
					//{
					//	IDELineMarker.SetWalkerPosition(currentLineNumber);
					//}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					int lineNumber = processor.CurrentSource.FromRow;
					SelectCodeWindowForAgent(agentProcessors[i].agent);
					InternalStopRunning(StopStatus.RuntimeError);
					PMWrapper.RaiseError(lineNumber + 1, e.Message);
				}

				//variableWindow.UpdateList(processors);
			}

			if (agentProcessors.Count == 0)
			{
				enabled = false;
				Debug.Log("COMPILER: ALL PROCESSES ENDED");
			}
			else if (ended > 0)
			{
				Debug.Log(
					$"COMPILER: {ended} {(ended == 1 ? "PROCESS" : "PROCESSES")} ENDED, {agentProcessors.Count} STILL RUNNING");
			}
		}

		void SelectCodeWindowForAgent(Agent agent)
		{
			AgentBank.main.SelectAgent(agent);
			configMenuList.SelectItem(configMenuList.menuItems
				.OfType<ConfigCodeWindow>()
				.First()
			);
			Canvas.ForceUpdateCanvases();
		}

		public void StartRunning()
		{
			foreach (IPMPreCompilerStarted ev in UISingleton.FindInterfaces<IPMPreCompilerStarted>())
			{
				ev.OnPMPreCompilerStarted();
			}

			CompilerSettings compilerSettings = GetSettings();
			agentProcessors.Clear();

			foreach (Agent agent in bank.agents)
			{
				try
				{
					var compiler = new PyCompiler {
						Settings = compilerSettings
					};
					compiler.Compile(agent.code);

					for (int i = agent.instances.Count - 1; i >= 0; i--)
					{
						AgentInstance agentInstance = agent.instances[i];
						if (!agentInstance)
						{
							agent.instances.RemoveAt(i);
							Debug.LogWarning(
								$"Agent instance at index {i} for agent {agent.name} was null. Removing it.", this);
							continue;
						}

						IProcessor processor = compiler.Compile(string.Empty);
						processor.AddBuiltin(
							BUILTIN_FUNCTIONS
						);
						processor.AddBuiltin(
							AgentBank.GetAgentFunctions(agentInstance)
						);

						agentProcessors.Add(new AgentProcessor(agent, processor));
					}

					//IDELineMarker.SetWalkerPosition(currentLineNumber);
				}
				catch (SyntaxException e)
				{
					Debug.LogException(e);
					SelectCodeWindowForAgent(agent);
					PMWrapper.RaiseError(e.SourceReference.FromColumn + 1, e.Message);
					return;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					SelectCodeWindowForAgent(agent);
					PMWrapper.RaiseError(0, e.Message);
					return;
				}
			}

			enabled = true;
			isPaused = false;
			isRunning = true;

			// Call event
			Debug.Log($"COMPILER: STARTED {agentProcessors.Count} PROCESSORS");
			foreach (IPMCompilerStarted ev in UISingleton.FindInterfaces<IPMCompilerStarted>())
			{
				ev.OnPMCompilerStarted();
			}
		}

		public void PauseRunning()
		{
			if (!isRunning || isPaused)
			{
				return;
			}

			isPaused = true;
			enabled = false;

			// Call event
			Debug.Log("COMPILER: RESUMED");
			foreach (IPMCompilerUserUnpaused ev in UISingleton.FindInterfaces<IPMCompilerUserUnpaused>())
			{
				ev.OnPMCompilerUserUnpaused();
			}
		}

		public void ResumeRunning()
		{
			if (agentProcessors == null || !isPaused)
			{
				return;
			}

			isPaused = false;
			enabled = true;

			// Call event
			Debug.Log("COMPILER: PAUSED");
			foreach (IPMCompilerUserPaused ev in UISingleton.FindInterfaces<IPMCompilerUserPaused>())
			{
				ev.OnPMCompilerUserPaused();
			}
		}

		public void StopRunning(StopStatus stopStatus = StopStatus.CodeForced)
		{
			if (!isRunning)
			{
				return;
			}

			InternalStopRunning(stopStatus);
		}

		void InternalStopRunning(StopStatus stopStatus)
		{
			enabled = false;
			agentProcessors.Clear();
			isPaused = false;
			isRunning = false;

			// Call event
			Debug.Log("COMPILER: STOPPED");
			foreach (IPMCompilerStopped ev in UISingleton.FindInterfaces<IPMCompilerStopped>())
			{
				ev.OnPMCompilerStopped(stopStatus);
			}
		}

		static CompilerSettings GetSettings()
		{
			CompilerSettings settings = CompilerSettings.DefaultSettings;

			settings.BreakOn |= BreakCause.LoopBlockEnd;

			return settings;
		}

		struct AgentProcessor
		{
			public readonly Agent agent;
			public readonly IProcessor processor;

			public AgentProcessor(Agent agent, IProcessor processor)
			{
				this.agent = agent;
				this.processor = processor;
			}
		}
	}
}
