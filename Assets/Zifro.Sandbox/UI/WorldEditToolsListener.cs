using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zifro.Sandbox.UI.WorldEdit;

namespace Zifro.Sandbox.UI
{
	public class WorldEditToolsListener : MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler
	{
		public WorldEditToolsSelector toolSelector;

		IToolScreenEnter[] pointerEnterHandlers;
		IToolScreenExit[] pointerExitHandlers;

		void Awake()
		{
			Debug.Assert(toolSelector, "Tool Selector not assigned.", this);
		}

		void OnEnable()
		{
			pointerEnterHandlers = toolSelector.tools.OfType<IToolScreenEnter>().ToArray();
			pointerExitHandlers = toolSelector.tools.OfType<IToolScreenExit>().ToArray();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			foreach (IToolScreenEnter handler in pointerEnterHandlers)
			{
				handler.OnScreenEnter(eventData);
			}
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			foreach (IToolScreenExit handler in pointerExitHandlers)
			{
				handler.OnScreenExit(eventData);
			}
		}
	}
}
