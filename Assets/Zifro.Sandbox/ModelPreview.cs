using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class ModelPreview : MonoBehaviour
	{
		public RenderTexture texture;
		public ModelTextureRegenerateSettings regenerateSettings;

		[Space]
		public GameObject modelPrefab;
		public GameObject modelInstance;
		public Camera renderCamera;

		[SerializeField]
		bool continuousRendering;

		void Start()
		{
			Debug.Assert(modelPrefab, $"{nameof(modelPrefab)} not defined in {name}.", this);
			Debug.Assert(modelInstance, $"{nameof(modelInstance)} not defined in {name}.", this);
			Debug.Assert(renderCamera, $"{nameof(renderCamera)} not defined in {name}.", this);

			renderCamera.enabled = continuousRendering;
			if (!texture)
			{
				RegenerateTextureFromSettings();
			}

			name = $"Model preview '{modelPrefab.name}'";
		}

#if UNITY_EDITOR

		void OnValidate()
		{
			if (renderCamera)
			{
				renderCamera.enabled = continuousRendering;
			}
		}

#endif

		public void RegenerateTextureFromSettings()
		{
			Debug.Assert(regenerateSettings, $"{nameof(regenerateSettings)} not defined in {name}.", this);

			RegenerateTexture(
				regenerateSettings.textureWidth,
				regenerateSettings.textureHeight,
				regenerateSettings.textureDepth
			);
		}

		public void RegenerateTexture(int textureWidth, int textureHeight, int textureDepth)
		{
			texture = new RenderTexture(
				textureWidth,
				textureHeight,
				textureDepth,
				RenderTextureFormat.Default
			) {
				name = $"Preview render for '{modelPrefab.name}'"
			};

			renderCamera.forceIntoRenderTexture = true;
			renderCamera.targetTexture = texture;

			RenderOntoTexture();
		}

		public void RenderOntoTexture()
		{
			renderCamera.Render();
		}

		public void EnableContinuousRendering()
		{
			continuousRendering = true;
			renderCamera.enabled = true;
		}

		public void DisableContinuousRendering()
		{
			continuousRendering = false;
			renderCamera.enabled = false;
		}
	}
}
