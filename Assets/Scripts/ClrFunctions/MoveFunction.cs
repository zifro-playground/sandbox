using System.Collections;
using System.Collections.Generic;
using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

public class MoveFunction : ClrFunction
{
	readonly Direction direction;

	public MoveFunction(string name, Direction direction) : base(name)
	{
		this.direction = direction;
	}

	public override IScriptType Invoke(params IScriptType[] arguments)
	{
		var player = Object.FindObjectOfType<PlayerController>();

		if (!player)
		{
			PMWrapper.RaiseError("Hittade ingen spelare.");
		}

		player.Walk(direction);

		return null;
	}
}
