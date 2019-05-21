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
	void OnPMAgentSelected(Agent selectedAgent);
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
	void OnPMAgentDeselected(Agent deselectedAgent);
}

/// <summary>
/// Event that fires when all agents are deselected in the menu.
/// Means that when called, no agent is selected.
/// Fires just after the individual deselection event <see cref="IPMAgentDeselected"/>.
/// <para>Must be applied to object inheriting from <see cref="Object"/></para>
/// </summary>
public interface IPMAgentAllDeselected
{
	/// <summary>
	/// Fires when all agents are deselected in the menu.
	/// Means that when called, no agent is selected.
	/// Fires just after the individual deselection event <see cref="IPMAgentDeselected"/>.
	/// </summary>
	void OnPMAgentAllDeselected(Agent deselectedAgent);
}

/// <summary>
/// Event that fires when an agent is updated. This can be the name, model, etc.
/// <para>Must be applied to object inheriting from <see cref="Object"/></para>
/// </summary>
public interface IPMAgentUpdated
{
	/// <summary>
	/// Fires when an agent is updated. This can be the name, model, etc.
	/// </summary>
	void OnPMAgentUpdated(Agent updatedAgent);
}
