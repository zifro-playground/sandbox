using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace Zifro.Sandbox.ClrFunctions
{
	public class GetKeyDownFunction : ClrFunction
	{
		readonly KeyCode key;

		public GetKeyDownFunction(string name, KeyCode key) : base(name)
		{
			this.key = key;
		}

		public override IScriptType Invoke(params IScriptType[] arguments)
		{
			return Processor.Factory.Create(Input.GetKey(key));
		}
	}
}
