using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class RotateFunction : ClrFunction
	{
		readonly Rotation rotation;
		readonly PlayerController player;

		public RotateFunction(string name, Rotation rotation) : base(name)
		{
			this.rotation = rotation;
			player = Object.FindObjectOfType<PlayerController>();

			Debug.Assert(player, "Unable to find " + nameof(PlayerController));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			switch (arguments.Length > 0 ? arguments[0] : null)
			{
			case IScriptDouble d:
				player.Rotate(rotation, (float)d.Value);
				break;
			case IScriptInteger i:
				player.Rotate(rotation, i.Value);
				break;
			default:
				player.Rotate(rotation);
				break;
			}

			return null;
		}
	}
}
