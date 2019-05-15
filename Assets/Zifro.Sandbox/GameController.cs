using UnityEngine;
using Zifro.Sandbox.ClrFunctions;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class GameController : MonoBehaviour
	{
		void Awake()
		{
			PMWrapper.codeRowsLimit = 1000;
			PMWrapper.AutoSetSmartButtons();

			Time.fixedDeltaTime = 1f / FractionVector3.SCALE;
			//Time.fixedDeltaTime = 1;
		}
	}
}
