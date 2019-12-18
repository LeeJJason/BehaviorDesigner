using System;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x0200001B RID: 27
	[InitializeOnLoad]
	public class HierarchyIcon : ScriptableObject
	{
		// Token: 0x060001F1 RID: 497 RVA: 0x000122BC File Offset: 0x000104BC
		static HierarchyIcon()
		{
			if (HierarchyIcon.icon != null)
			{
				EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(HierarchyIcon.HierarchyWindowItemOnGUI));
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00012318 File Offset: 0x00010518
		private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.ShowHierarchyIcon))
			{
				GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
				if (gameObject != null && gameObject.GetComponent<Behavior>() != null)
				{
                    Rect rect = selectionRect;
                    rect.x = rect.width + (selectionRect.x - 16f);
					rect.width = 16f;
					rect.height = 16f;
					GUI.DrawTexture(rect, HierarchyIcon.icon);
				}
			}
		}

		// Token: 0x0400012C RID: 300
		private static Texture2D icon = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/Behavior Designer Hier Icon.png", typeof(Texture2D)) as Texture2D;
	}
}
