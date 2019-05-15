using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zifro.Sandbox.Utility
{
	public class InputFieldWithOnSubmit : InputField
	{
		public SubmitEvent onSubmit;

		protected override void OnEnable()
		{
			base.OnEnable();
			
			onEndEdit.AddListener(OnEndEdit);
		}

		void OnEndEdit(string arg0)
		{
			if (EventSystem.current.alreadySelecting)
			{
				return;
			}

			onSubmit.Invoke(arg0);
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current){selectedObject = gameObject}, ExecuteEvents.submitHandler);
		}
	}
}
