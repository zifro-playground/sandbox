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
		static readonly IReadOnlyCollection<IEmbeddedType> BUILTIN_FUNCTIONS = new IClrFunction[] {
			new AbsoluteValue(),
			new ConvertToBinary(),
			new ConvertToHexadecimal(),
			new LengthOf(),
			new RoundedValue(),
			new MinimumValue(),
			new MaximumValue(),
			new GetTime()
		};

		IProcessor processor;
		int lastLineNumber;

		public bool isRunning => processor?.State == ProcessState.Running ||
		                         processor?.State == ProcessState.Yielded;

		public bool isPaused { get; private set; }

		public int currentLineNumber => processor?.CurrentSource.IsFromClr == false
			? lastLineNumber = processor.CurrentSource.FromRow
			: lastLineNumber;

		public VariableWindow variableWindow;

		void Start()
		{
			Debug.Assert(variableWindow, "Variable window undefined.");
		}

		void FixedUpdate()
		{
			if (processor == null||
			    processor.State == ProcessState.Ended ||
			    processor.State == ProcessState.Error)
			{
				enabled = false;
				return;
			}

			if (processor.State == ProcessState.Yielded)
			{
				return;
			}

			try
			{
				WalkStatus result = processor.Walk();

				if (result == WalkStatus.Ended)
				{
					enabled = false;
				}
				else
				{
					IDELineMarker.SetWalkerPosition(currentLineNumber);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				int lineNumber = currentLineNumber;
				InternalStopRunning(StopStatus.RuntimeError);
				PMWrapper.RaiseError(lineNumber, e.Message);
			}

			variableWindow.UpdateList(processor);
		}

		public void StartRunning()
		{
			try
			{
				processor = new PyCompiler {
					Settings = GetSettings()
				}.Compile(PMWrapper.fullCode);

				processor.AddBuiltin(
					BUILTIN_FUNCTIONS.Concat(UISingleton.instance.walker.addedFunctions).ToArray()
				);

				IDELineMarker.SetWalkerPosition(currentLineNumber);
			}
			catch (SyntaxException e)
			{
				Debug.LogException(e);
				PMWrapper.RaiseError(e.SourceReference.FromColumn, e.Message);
				return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				PMWrapper.RaiseError(currentLineNumber, e.Message);
				return;
			}

			enabled = true;
			isPaused = false;

			// Call event
			Debug.Log("COMPILER: STARTED");
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
			if (processor == null || !isPaused)
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
			if (processor == null)
			{
				return;
			}

			InternalStopRunning(stopStatus);
		}

		private void InternalStopRunning(StopStatus stopStatus)
		{
			enabled = false;
			processor = null;
			isPaused = false;

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
