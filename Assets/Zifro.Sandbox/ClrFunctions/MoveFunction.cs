using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

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
			if (arguments.Length > 0 && arguments[0].TryConvert(out float scale))
			{
				player.Walk(direction, scale);
			}
			else
			{
				player.Walk(direction);
			}

			return null;
		}
	}
}
