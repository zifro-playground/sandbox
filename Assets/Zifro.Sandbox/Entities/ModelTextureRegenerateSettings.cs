using UnityEngine;

namespace Zifro.Sandbox.Entities
{
	[CreateAssetMenu(menuName = "Zifro Sandbox/Model Regenerate Texture Settings", order = 10)]
	public class ModelTextureRegenerateSettings : ScriptableObject
	{
		public int textureWidth = 64;
		public int textureHeight = 64;
		public int textureDepth = 1;
	}
}
