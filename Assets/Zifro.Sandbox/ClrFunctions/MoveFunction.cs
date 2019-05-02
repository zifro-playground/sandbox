using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace Zifro.Sandbox.ClrFunctions
{
	public class MoveFunction : ClrFunction
	{
		readonly Direction direction;

		public MoveFunction(string name, Direction direction) : base(name)
		{
			this.direction = direction;
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			PlayerController player = Object.FindObjectOfType<PlayerController>();

			if (!player)
			{
				PMWrapper.RaiseError("Hittade ingen spelare.");
			}

			player.Walk(direction);

			return null;
		}
	}
}
