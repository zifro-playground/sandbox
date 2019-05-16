using UnityEngine;

namespace Zifro.Sandbox.Utility
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class MimicRectTransform : MonoBehaviour
	{
		public RectTransform target;

		RectTransform rect;
		DrivenRectTransformTracker tracker;

		void OnEnable()
		{
			rect = (RectTransform)transform;

			tracker = new DrivenRectTransformTracker();
			tracker.Clear();

			const DrivenTransformProperties drivenProperties =
				DrivenTransformProperties.Pivot |
				DrivenTransformProperties.AnchoredPosition |
				DrivenTransformProperties.SizeDelta;

			tracker.Add(this, rect, drivenProperties);
		}

		void OnDisable()
		{
			tracker.Clear();
		}

		void Update()
		{
			if (!target)
			{
				return;
			}

			rect.position = target.position;
			rect.pivot = target.pivot;
			Rect targetRect = target.rect;
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRect.width);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRect.height);
		}
	}
}
