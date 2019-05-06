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
				new IsBlockedFunction("blockerad_framåt", Direction.Forward),
				new IsBlockedFunction("blockerad_bakåt", Direction.Backward),
				new IsBlockedFunction("blockerad_höger", Direction.Right),
				new IsBlockedFunction("blockerad_vänster", Direction.Left),
				new IsBlockedFunction("blockerad_upp", Direction.Up),
				new IsBlockedFunction("blockerad_ner", Direction.Down),

				new GetKeyDownFunction("knapp_höger", KeyCode.RightArrow),
				new GetKeyDownFunction("knapp_vänster", KeyCode.LeftArrow),
				new GetKeyDownFunction("knapp_upp", KeyCode.UpArrow),
				new GetKeyDownFunction("knapp_ner", KeyCode.DownArrow),
				new GetKeyDownFunction("knapp_mellanslag", KeyCode.Space),

				new RotateFunction("sväng_höger", Rotation.Right),
				new RotateFunction("sväng_vänster", Rotation.Left),

				new MoveFunction("gå_framåt", Direction.Forward),
				new MoveFunction("gå_bakåt", Direction.Backward),
				new MoveFunction("gå_höger", Direction.Right),
				new MoveFunction("gå_vänster", Direction.Left),
				new MoveFunction("gå_upp", Direction.Up),
				new MoveFunction("gå_ner", Direction.Down)

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
