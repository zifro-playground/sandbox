﻿using Mellis;
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

			if (!player)
			{
				PMWrapper.RaiseError("Hittade ingen spelare.");
			}
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			player.Walk(direction);

			return null;
		}
	}
}