using UnityEngine;
using Zifro.Sandbox.ClrFunctions;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class GameController : MonoBehaviour
	{
		void Awake()
		{
			PMWrapper.SetCompilerFunctions(
				new RotateFunction("sväng_höger", Rotation.Right),
				new RotateFunction("sväng_vänster", Rotation.Left),
				new MoveFunction("gå_framåt", Direction.Forward),
				new MoveFunction("gå_bakåt", Direction.Backward),
				new MoveFunction("gå_höger", Direction.Right),
				new MoveFunction("gå_vänster", Direction.Left)
				//new MoveFunction("gå_norr", Direction.North),
				//new MoveFunction("gå_väst", Direction.West),
				//new MoveFunction("gå_syd", Direction.South),
				//new MoveFunction("gå_öst", Direction.East)
			);

			PMWrapper.AutoSetSmartButtons();

			Time.fixedDeltaTime = 1f / FractionVector3.SCALE;
			//Time.fixedDeltaTime = 1;
		}
	}
}
