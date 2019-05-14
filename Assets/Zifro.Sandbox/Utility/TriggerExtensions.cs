using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Zifro.Sandbox.Utility
{
	public static class TriggerExtensions
	{
		public static void AddTrigger(this EventTrigger trigger, EventTriggerType entryType, UnityAction<BaseEventData> callback)
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
	}
}
