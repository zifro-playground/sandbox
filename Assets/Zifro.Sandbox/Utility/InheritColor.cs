using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Zifro.Sandbox.Utility
{
	[AddComponentMenu("UI/Effects/InheritColor")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public class InheritColor : MonoBehaviour
	{
		public Graphic target;

		[Range(0, 1)]
		public float rgbMultiplier = .5f;
		[Range(0, 1)]
		public float alphaMultiplier = 1;

		Graphic graphic;

		void Awake()
		{
			graphic = GetComponent<Graphic>();
		}

		void Update()
		{
			if (!target)
			{
				return;
			}

			if (!graphic)
			{
				graphic = GetComponent<Graphic>();
				if (!graphic)
				{
					return;
				}
			}

			Color targetColor = target.color * target.canvasRenderer.GetColor();
			Color color = Lerp(graphic.color, targetColor, rgbMultiplier, alphaMultiplier);
			graphic.canvasRenderer.SetColor(color);
		}

		void OnDisable()
		{

			Graphic self = GetComponent<Graphic>();
			if (self)
			{
				self.canvasRenderer.SetColor(Color.white);
			}
		}


		static Color Lerp(Color a, Color b, float rgbT, float aT)
		{
			rgbT = Mathf.Clamp01(rgbT);
			aT = Mathf.Clamp01(aT);
			return new Color(a.r + (b.r - a.r) * rgbT, a.g + (b.g - a.g) * rgbT, a.b + (b.b - a.b) * rgbT, a.a + (b.a - a.a) * aT);
		}
	}
}
