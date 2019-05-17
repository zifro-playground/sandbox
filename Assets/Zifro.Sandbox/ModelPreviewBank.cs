using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox
{
	public class ModelPreviewBank : MonoBehaviour
	{
		public static ModelPreviewBank main;

		public GameObject modelPreviewPrefab;
		public float previewSpacing = 3;
		public List<GameObject> modelPrefabs = new List<GameObject>();
		public List<ModelPreview> previews = new List<ModelPreview>();

		void OnEnable()
		{
			Debug.Assert(!main,
				$"There are multiple agent bank instances. '{(main ? main.name : string.Empty)}' and '{name}'.", this);
			main = this;
		}

		void Start()
		{
			Debug.Assert(modelPreviewPrefab, $"{nameof(modelPreviewPrefab)} is not assigned in '{name}'.", this);

			for (int i = previews.Count - 1; i >= 0; i--)
			{
				ModelPreview model = previews[i];

				if (model == null)
				{
					Debug.LogWarning($"Model preview at index {i} was null. Removing.", this);
					previews.RemoveAt(i);
					continue;
				}

				Debug.Assert(model.renderCamera, $"{nameof(model.renderCamera)} is not assigned for model preview '{model.name}' (at index {i}).", this);
			}

			for (int i = modelPrefabs.Count - 1; i >= 0; i--)
			{
				GameObject model = modelPrefabs[i];

				if (model == null)
				{
					Debug.LogWarning($"Model prefab at index {i} was null. Removing.", this);
					previews.RemoveAt(i);
				}
			}

			// Create missing previews
			foreach (GameObject modelPrefab in modelPrefabs.Except(previews.Select(o => o.modelPrefab)))
			{
				CreatePreview(modelPrefab);
			}
		}

		void Reset()
		{
			previews.AddRange(GetComponentsInChildren<ModelPreview>()
				.Except(previews));
		}

		public ModelPreview CreatePreview(GameObject modelPrefab)
		{
			Transform parent = transform;
			Vector3 position = parent.position + new Vector3(0, 0, -previews.Count * previewSpacing);
			Quaternion rotation = parent.rotation;

			// Instantiate
			GameObject previewClone = Instantiate(modelPreviewPrefab, position, rotation, parent);
			GameObject modelClone = Instantiate(modelPrefab, position, rotation, previewClone.transform);

			// Setup preview
			ModelPreview preview = previewClone.GetComponent<ModelPreview>();
			preview.previewInstance = modelClone;
			preview.modelPrefab = modelPrefab;
			previews.Add(preview);

			return preview;
		}

		[Pure]
		public Texture GetTexture(GameObject model)
		{
			Debug.Assert(model, "Model cannot be null.", this);

			ModelPreview modelPreview = previews.FirstOrDefault(o => o.modelPrefab == model);

			if (!modelPreview)
			{
				return null;
			}

			if (!modelPreview.texture)
			{
				modelPreview.RegenerateTextureFromSettings();
			}

			return modelPreview.texture;
		}

		public Texture GetOrCreateTexture(GameObject model)
		{
			Debug.Assert(model, "Model cannot be null.", this);

			ModelPreview modelPreview = previews.FirstOrDefault(o => o.modelPrefab == model);

			if (!modelPreview)
			{
				modelPreview = CreatePreview(model);
			}

			if (!modelPreview.texture)
			{
				modelPreview.RegenerateTextureFromSettings();
			}

			return modelPreview.texture;
		}

		[Pure]
		public ModelPreview GetPreview(GameObject model)
		{
			Debug.Assert(model, "Model cannot be null.", this);

			return previews.FirstOrDefault(o => o.modelPrefab == model);
		}

		public ModelPreview GetOrCreatePreview(GameObject model)
		{
			Debug.Assert(model, "Model cannot be null.", this);

			ModelPreview preview = previews.FirstOrDefault(o => o.modelPrefab == model);

			if (!preview)
			{
				return CreatePreview(model);
			}

			return preview;
		}
	}
}
