using Zifro.Sandbox.Entities;

/// <summary>
/// Event that fires when an agent is selected in the menu.
/// <para>Must be applied to object inheriting from <see cref="Object"/></para>
/// </summary>
public interface IPMAgentSelected
{
	/// <summary>
	/// Fires when an agent is selected in the menu.
	/// </summary>
	void OnPMAgentSelected(Agent agent);
}

/// <summary>
/// Event that fires when an agent is deselected in the menu.
/// Fires just before the selection event <see cref="IPMAgentSelected"/>.
/// <para>Must be applied to object inheriting from <see cref="Object"/></para>
/// </summary>
public interface IPMAgentDeselected
{
	/// <summary>
	/// Fires when an agent is deselected in the menu.
	/// Fires just before the selection event <see cref="IPMAgentSelected"/>.
	/// </summary>
	void OnPMAgentDeselected(Agent agent);
}
