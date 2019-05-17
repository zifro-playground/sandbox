using UnityEditor;
using UnityEngine;
using Zifro.Sandbox;

[CustomPreview(typeof(ModelPreview))]
public class CustomPreview_ModelPreview : ObjectPreview
{
	public override bool HasPreviewGUI()
	{
		return ((ModelPreview)target).texture;
	}

	public override void OnPreviewGUI(Rect r, GUIStyle background)
	{
		Texture texture = ((ModelPreview)target).texture;

		if (!texture)
		{
			return;
		}

		EditorGUI.DrawPreviewTexture(r, texture, null, ScaleMode.ScaleToFit);
	}
}
