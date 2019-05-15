using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class MoveFunction : ClrFunction
	{
		readonly Direction direction;
		readonly AgentInstance instance;

		public MoveFunction(string name, AgentInstance instance, Direction direction) : base(name)
		{
			this.direction = direction;
			this.instance = instance;

			Debug.Assert(instance, "Unable to find " + nameof(AgentInstance));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			switch (arguments.Length > 0 ? arguments[0] : null)
			{
			case IScriptDouble d:
				instance.Walk(direction, (float)d.Value);
				break;
			case IScriptInteger i:
				instance.Walk(direction, i.Value);
				break;
			default:
				instance.Walk(direction);
				break;
			}

			return null;
		}
	}
}
