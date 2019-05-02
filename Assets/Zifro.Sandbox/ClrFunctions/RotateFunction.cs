using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

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

			if (!player)
			{
				PMWrapper.RaiseError("Hittade ingen spelare.");
			}
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			player.Rotate(rotation);

			return null;
		}
	}
}
