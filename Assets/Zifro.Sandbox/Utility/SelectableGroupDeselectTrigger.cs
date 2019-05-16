using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zifro.Sandbox.Utility
{
	[AddComponentMenu("Event/Selectable Group Deselect Trigger")]
	public class SelectableGroupDeselectTrigger : MonoBehaviour
	{
		public List<Selectable> selectables;

		[Space]
		public EventTrigger.TriggerEvent onDeselect;

		readonly Dictionary<GameObject, UnityAction<BaseEventData>> registered =
			new Dictionary<GameObject, UnityAction<BaseEventData>>();

		void Reset()
		{
			selectables = GetComponentsInChildren<Selectable>().ToList();
		}

		void OnEnable()
		{
			foreach (Selectable child in selectables)
			{
				UnityAction<BaseEventData> callback = OnDeselect;
				child.AddTrigger(EventTriggerType.Deselect, callback);
				registered[child.gameObject] = callback;
			}
		}

		void OnDisable()
		{
			foreach (KeyValuePair<GameObject, UnityAction<BaseEventData>> pair in registered)
			{
				pair.Key.RemoveTrigger(EventTriggerType.Deselect, pair.Value);
			}
			registered.Clear();
		}

		void OnDeselect(BaseEventData eventData)
		{
			if (!selectables.Select(o => o.gameObject)
				.Contains(eventData.selectedObject))
			{
				// Not part of the gang anymore
				if (registered.ContainsKey(eventData.selectedObject))
				{
					eventData.selectedObject.RemoveTrigger(EventTriggerType.Deselect,
						registered[eventData.selectedObject]);
					registered.Remove(eventData.selectedObject);
				}
			}
			else
			{
				StartCoroutine(CheckNewSelectedObject(eventData));
			}
		}

		IEnumerator CheckNewSelectedObject(BaseEventData eventData)
		{
			yield return null;
			if (!selectables.Select(o => o.gameObject)
				.Contains(EventSystem.current.currentSelectedGameObject))
			{
				onDeselect.Invoke(eventData);
			}
		}
	}
}
