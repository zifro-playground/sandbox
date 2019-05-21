using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI
{
	public class MenuItem : MonoBehaviour
	{
		public Button button;

		[NonSerialized]
		public bool isSelected;

		protected void Start()
		{
			Debug.Assert(button, $"{nameof(button)} is not assigned for {name}.", this);
		}

		public virtual void OnMenuItemSelected(MenuItem lastItem)
		{
			// Do nothing
		}

		public virtual void OnMenuItemDeselected()
		{
			// Do nothing
		}
	}
}
