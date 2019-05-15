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
		public EventTrigger.TriggerEvent onDeselect;

		readonly HashSet<GameObject> selectables = new HashSet<GameObject>();

		void OnEnable()
		{
			selectables.Clear();
			Selectable[] children = GetComponentsInChildren<Selectable>(true);
			foreach (Selectable child in children)
			{
				child.AddTrigger(EventTriggerType.Deselect, OnDeselect);
				selectables.Add(child.gameObject);
			}
		}

		void OnDeselect(BaseEventData eventData)
		{
			StartCoroutine(CheckNewSelectedObject(eventData));
		}

		IEnumerator CheckNewSelectedObject(BaseEventData eventData)
		{
			yield return null;
			if (!selectables.Contains(EventSystem.current.currentSelectedGameObject))
			{
				onDeselect.Invoke(eventData);
			}
		}
	}
}
