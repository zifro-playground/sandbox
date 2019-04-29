using System.Collections;
using System.Collections.Generic;
using PM;
using UnityEngine;

public class GameController : MonoBehaviour
{
	void Awake()
	{
		PMWrapper.SetCompilerFunctions(
			new MoveFunction("gå_norr", Direction.North),
			new MoveFunction("gå_väst", Direction.West),
			new MoveFunction("gå_syd", Direction.South),
			new MoveFunction("gå_öst", Direction.East)
		);

		PMWrapper.AutoSetSmartButtons();
	}
}
