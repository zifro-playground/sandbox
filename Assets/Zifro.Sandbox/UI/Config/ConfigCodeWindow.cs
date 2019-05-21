using PM;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI.Config
{
	public class ConfigCodeWindow : ConfigMenuItem
	{
		public Selectable selectOnSelect;

		public override void OnMenuItemSelected(MenuItem lastItem)
		{
			base.OnMenuItemSelected(lastItem);

			if (selectOnSelect)
			{
				EventSystem.current.SetSelectedGameObject(selectOnSelect.gameObject);
			}
		}
	}
}
