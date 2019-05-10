using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class IsBlockedFunction : ClrFunction
	{
		readonly Direction direction;
		readonly PlayerController player;
		readonly GridWorld world;

		public IsBlockedFunction(string name, Direction direction) : base(name)
		{
			this.direction = direction;
			player = Object.FindObjectOfType<PlayerController>();
			world = Object.FindObjectOfType<GridWorld>();
			
			Debug.Assert(player, "Unable to find " + nameof(PlayerController));
			Debug.Assert(world, "Unable to find " + nameof(GridWorld));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			float scale;
			switch (arguments.Length > 0 ? arguments[0] : null)
			{
			case IScriptDouble d:
				scale = (float)d.Value;
				break;
			case IScriptInteger i:
				scale = i.Value;
				break;
			default:
				scale = 1;
				break;
			}

			Vector3 point = player.fractionPosition;

			if (scale.Equals(0f))
			{
				return Processor.Factory.Create(world.IsPointInBlock(point));
			}

			Vector3 vector = player.GetDirectionFraction(direction, scale);
			bool isBlocked = world.TryRaycastBlocks(point, vector, scale, out GridRaycastHit _);

			return Processor.Factory.Create(isBlocked);
		}
	}
}
