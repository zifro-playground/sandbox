using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class MoveUntilBlockedFunction : ClrFunction
	{
		readonly Direction direction;
		readonly PlayerController player;
		readonly GridWorld world;

		public MoveUntilBlockedFunction(string name, Direction direction) : base(name)
		{
			this.direction = direction;
			player = Object.FindObjectOfType<PlayerController>();
			world = Object.FindObjectOfType<GridWorld>();

			Debug.Assert(player, "Unable to find " + nameof(PlayerController));
			Debug.Assert(world, "Unable to find " + nameof(GridWorld));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			const float threshold = 0.5f;
			const float scaledThreshold = FractionVector3.SCALE * threshold;

			if (arguments.Length <= 0 || !arguments[0].TryConvert(out float scale))
			{
				scale = 1;
			}

			if (scale.Equals(0f))
			{
				return null;
			}

			Vector3 point = player.fractionPosition;
			Vector3 vector = player.GetDirectionFraction(direction, scale);

			bool isBlocked = world.TryRaycastBlocks(point, vector, scale * FractionVector3.SCALE_INVERSE + threshold, out RaycastHit hit);

			if (isBlocked)
			{
				float scaledDistance = hit.distance * FractionVector3.SCALE;
				if (scaledDistance > scaledThreshold)
				{
					player.Walk(direction, scaledDistance - scaledThreshold);
				}
			}
			else
			{
				player.Walk(direction, scale);
			}

			return null;
		}
	}
}
