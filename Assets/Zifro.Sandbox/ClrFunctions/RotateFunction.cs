using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class RotateFunction : ClrFunction
	{
		readonly Rotation rotation;
		readonly AgentInstance instance;

		public RotateFunction(string name, AgentInstance instance, Rotation rotation) : base(name)
		{
			this.rotation = rotation;
			this.instance = instance;

			Debug.Assert(instance, "Unable to find " + nameof(AgentInstance));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			switch (arguments.Length > 0 ? arguments[0] : null)
			{
			case IScriptDouble d:
				instance.Rotate(rotation, (float)d.Value);
				break;
			case IScriptInteger i:
				instance.Rotate(rotation, i.Value);
				break;
			default:
				instance.Rotate(rotation);
				break;
			}

			return null;
		}
	}
}
