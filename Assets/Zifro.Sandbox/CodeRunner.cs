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

		readonly List<IProcessor> processors = new List<IProcessor>();

		public bool isRunning { get; private set; }

		public bool isPaused { get; private set; }

		public VariableWindow variableWindow;

		AgentBank bank;

		void Awake()
		{
			Debug.Assert(variableWindow, "Variable window undefined.");
		}

		void OnEnable()
		{
			bank = AgentBank.main;
			Debug.Assert(bank, $"Unable to find main agent bank in '{name}'.");
		}

		void Start()
		{
			enabled = false;
		}

		void FixedUpdate()
		{
			int ended = 0;
			for (int i = processors.Count - 1; i >= 0; i--)
			{
				IProcessor processor = processors[i];
				switch (processor.State)
				{
				case ProcessState.Ended:
				case ProcessState.Error:
					processors.RemoveAt(i);
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
						processors.RemoveAt(i);
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
					InternalStopRunning(StopStatus.RuntimeError);
					PMWrapper.RaiseError(lineNumber + 1, e.Message);
				}

				//variableWindow.UpdateList(processors);
			}

			if (processors.Count == 0)
			{
				enabled = false;
				Debug.Log("COMPILER: ALL PROCESSES ENDED");
			}
			else if (ended > 0)
			{
				Debug.Log($"COMPILER: {ended} {(ended == 1 ? "PROCESS" : "PROCESSES")} ENDED, {processors.Count} STILL RUNNING");
			}
		}

		public void StartRunning()
		{
			foreach (IPMPreCompilerStarted ev in UISingleton.FindInterfaces<IPMPreCompilerStarted>())
			{
				ev.OnPMPreCompilerStarted();
			}

			try
			{
				CompilerSettings compilerSettings = GetSettings();
				processors.Clear();

				foreach (Agent agent in bank.agents)
				{
					var compiler = new PyCompiler {
						Settings = compilerSettings
					};
					compiler.Compile(agent.code);

					foreach (AgentInstance agentInstance in agent.instances)
					{
						IProcessor processor = compiler.Compile(string.Empty);
						processor.AddBuiltin(
							BUILTIN_FUNCTIONS
						);
						processor.AddBuiltin(
							AgentBank.GetAgentFunctions(agentInstance)
						);

						processors.Add(processor);
					}
				}

				//IDELineMarker.SetWalkerPosition(currentLineNumber);
			}
			catch (SyntaxException e)
			{
				Debug.LogException(e);
				PMWrapper.RaiseError(e.SourceReference.FromColumn + 1, e.Message);
				return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				PMWrapper.RaiseError(0, e.Message);
				return;
			}

			enabled = true;
			isPaused = false;
			isRunning = true;

			// Call event
			Debug.Log($"COMPILER: STARTED {processors.Count} PROCESSORS");
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
			if (processors == null || !isPaused)
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
			processors.Clear();
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
	}
}
