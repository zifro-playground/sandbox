using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zifro.Sandbox.Utility
{
	public static class TriggerExtensions
	{
		public static void AddTrigger(
			this EventTrigger trigger,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger.Entry entry = trigger.triggers.FirstOrDefault(o => o.eventID == entryType);

			if (entry == null)
			{
				entry = new EventTrigger.Entry {
					eventID = entryType,
					callback = new EventTrigger.TriggerEvent()
				};
				trigger.triggers.Add(entry);
			}

			entry.callback.AddListener(callback);
		}

		public static void AddTrigger(
			this GameObject gameObject,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
			if (!trigger)
			{
				trigger = gameObject.AddComponent<EventTrigger>();
			}

			trigger.AddTrigger(entryType, callback);
		}

		public static void AddTrigger(
			this Selectable selectable,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger trigger = selectable.GetComponent<EventTrigger>();
			if (!trigger)
			{
				trigger = selectable.gameObject.AddComponent<EventTrigger>();
			}

			trigger.AddTrigger(entryType, callback);
		}

		public static void RemoveTrigger(
			this EventTrigger trigger,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger.Entry entry = trigger.triggers.FirstOrDefault(o => o.eventID == entryType);

			// Don't actually know if it gets removed
			entry?.callback.RemoveListener(callback);
		}

		public static void RemoveTrigger(
			this GameObject gameObject,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
			if (!trigger)
			{
				return;
			}

			trigger.RemoveTrigger(entryType, callback);
		}

		public static void RemoveTrigger(
			this Selectable selectable,
			EventTriggerType entryType,
			UnityAction<BaseEventData> callback)
		{
			EventTrigger trigger = selectable.GetComponent<EventTrigger>();
			if (!trigger)
			{
				return;
			}

			trigger.RemoveTrigger(entryType, callback);
		}
	}
}
