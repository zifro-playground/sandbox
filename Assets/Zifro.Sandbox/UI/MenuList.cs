using System;
using System.Collections.Generic;
using System.Linq;
using PM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI
{
	public abstract class MenuList<T> : MonoBehaviour,
		IPMCompilerStarted,
		IPMCompilerStopped
		where T: MenuItem
	{
		public T currentItem;
		public List<T> menuItems = new List<T>();

		[NonSerialized]
		public bool isSelecting;

		public event Action<T> SelectedItem;
		public event Action<T> DeselectedItem;
		public event Action DeselectedAllItems;

#if UNITY_EDITOR
		protected void OnValidate()
		{
			if (UnityEditor.EditorApplication.isPlaying)
			{
				return;
			}

			if (currentItem && !menuItems.Contains(currentItem))
			{
				menuItems.Add(currentItem);
			}

			for (int i = menuItems.Count - 1; i >= 0; i--)
			{
				T tool = menuItems[i];

				if (!tool)
				{
					Debug.LogAssertion($"Menu item at index {i} was null. Removing it from list.");
					menuItems.RemoveAt(i);
					continue;
				}

				if (!tool.button)
				{
					tool.button = tool.GetComponent<Button>();
					if (!tool.button)
					{
						Debug.LogAssertion($"Unable to find button for {tool.name}. Removing it from list.");
						menuItems.RemoveAt(i);
						continue;
					}
				}

				tool.button.interactable = tool != currentItem;
			}
		}
#endif
		protected void Start()
		{
			Debug.Assert(menuItems.Count > 0, $"No tools found in {name}.", this);

			foreach (T tool in menuItems)
			{
				// Initialize tool
				Debug.Assert(tool.button, $"Button for tool {tool.GetType().Name} is not assigned in \"{tool.name}\"", tool);

				// Register events
				tool.button.onClick.AddListener(() => SelectItem(tool));
			}

			SelectItemInternal(currentItem ? currentItem : menuItems.First(), true);
		}

		public void SelectItem(T selectThis)
		{
			SelectItemInternal(selectThis, false);
		}

		void SelectItemInternal(T selectThis, bool force)
		{
			Debug.Assert(selectThis, "Tool cannot be null.");

			if (currentItem == selectThis && !force)
			{
				return;
			}

			isSelecting = true;

			foreach (T tool in menuItems)
			{
				if (tool == selectThis)
				{
					continue;
				}

				if (!tool.isSelected)
				{
					continue;
				}

				tool.button.interactable = true;
				tool.isSelected = false;
				OnDeselectedItem(tool);
				tool.OnMenuItemDeselected();
			}

			EventSystem.current.SetSelectedGameObject(null);
			T lastItem = currentItem;
			currentItem = selectThis;
			currentItem.button.interactable = false;
			currentItem.isSelected = true;
			OnSelectedMenuItem(lastItem, currentItem);
			currentItem.OnMenuItemSelected(lastItem);

			isSelecting = false;
		}

		public void DeselectTool()
		{
			if (!currentItem)
			{
				return;
			}

			currentItem.button.interactable = true;
			currentItem.isSelected = false;
			OnDeselectedItem(currentItem);
			OnDeselectedAllItems();
			currentItem.OnMenuItemDeselected();
			currentItem = null;
		}

		public void DeselectToolWithoutUIUpdate()
		{
			if (!currentItem)
			{
				return;
			}

			currentItem.isSelected = false;
			OnDeselectedItem(currentItem);
			OnDeselectedAllItems();
			currentItem.OnMenuItemDeselected();
			currentItem = null;
		}

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			DeselectTool();
			foreach (T tool in menuItems)
			{
				tool.button.interactable = false;
			}

			enabled = false;
		}

		void IPMCompilerStopped.OnPMCompilerStopped(StopStatus status)
		{
			foreach (T tool in menuItems)
			{
				if (currentItem != tool)
				{
					tool.button.interactable = true;
				}
			}

			enabled = true;
		}

		protected virtual void OnSelectedMenuItem(T lastItem, T item)
		{
			SelectedItem?.Invoke(item);
		}

		protected virtual void OnDeselectedItem(T item)
		{
			DeselectedItem?.Invoke(item);
		}

		protected virtual void OnDeselectedAllItems()
		{
			DeselectedAllItems?.Invoke();
		}
	}
}
