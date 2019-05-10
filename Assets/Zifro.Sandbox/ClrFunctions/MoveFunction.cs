using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class MoveFunction : ClrFunction
	{
		readonly Direction direction;
		readonly PlayerController player;

		public MoveFunction(string name, Direction direction) : base(name)
		{
			this.direction = direction;
			player = Object.FindObjectOfType<PlayerController>();

			Debug.Assert(player, "Unable to find " + nameof(PlayerController));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			switch (arguments.Length > 0 ? arguments[0] : null)
			{
			case IScriptDouble d:
				player.Walk(direction, (float)d.Value);
				break;
			case IScriptInteger i:
				player.Walk(direction, i.Value);
				break;
			default:
				player.Walk(direction);
				break;
			}

			return null;
		}
	}
}
