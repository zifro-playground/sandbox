using UnityEngine;

/// <summary>
/// Event that fires just before code is compiled.
/// <para>Must be applied to object inheriting from <see cref="Object"/></para>
/// </summary>
public interface IPMPreCompilerStarted
{
	/// <summary>
	/// Fires just before code is compiled.
	/// </summary>
	void OnPMPreCompilerStarted();
}
