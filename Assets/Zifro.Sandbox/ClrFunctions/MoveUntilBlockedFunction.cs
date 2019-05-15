using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.ClrFunctions
{
	public class MoveUntilBlockedFunction : ClrFunction
	{
		readonly Direction direction;
		readonly AgentInstance instance;
		readonly GridWorld world;

		public MoveUntilBlockedFunction(string name, AgentInstance instance, Direction direction) : base(name)
		{
			this.direction = direction;
			this.instance = instance;
			world = Object.FindObjectOfType<GridWorld>();

			Debug.Assert(instance, "Unable to find " + nameof(AgentInstance));
			Debug.Assert(world, "Unable to find " + nameof(GridWorld));
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			const float threshold = 0.5f;
			const float scaledThreshold = FractionVector3.SCALE * threshold;

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

			if (scale.Equals(0f))
			{
				return null;
			}

			Vector3 point = instance.fractionPosition;
			Vector3 vector = instance.GetDirectionFraction(direction, scale);

			bool isBlocked = world.TryRaycastBlocks(point, vector, scale * FractionVector3.SCALE_INVERSE + threshold, out GridRaycastHit hit);

			if (isBlocked)
			{
				float scaledDistance = hit.distance * FractionVector3.SCALE;
				if (scaledDistance > scaledThreshold)
				{
					instance.Walk(direction, scaledDistance - scaledThreshold);
				}
			}
			else
			{
				instance.Walk(direction, scale);
			}

			return null;
		}
	}
}
