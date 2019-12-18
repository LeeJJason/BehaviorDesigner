using System;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000017 RID: 23
	[InitializeOnLoad]
	public class GizmoManager
	{
		// Token: 0x0600019B RID: 411 RVA: 0x0000ECE4 File Offset: 0x0000CEE4
		static GizmoManager()
		{
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(GizmoManager.HierarchyChange));
			if (!Application.isPlaying)
			{
				GizmoManager.UpdateAllGizmos();
				EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(GizmoManager.UpdateAllGizmos));
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000ED54 File Offset: 0x0000CF54
		public static void UpdateAllGizmos()
		{
			Behavior[] array = Object.FindObjectsOfType<Behavior>();
			for (int i = 0; i < array.Length; i++)
			{
				GizmoManager.UpdateGizmo(array[i]);
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000ED84 File Offset: 0x0000CF84
		public static void UpdateGizmo(Behavior behavior)
		{
			behavior.gizmoViewMode = (Behavior.GizmoViewMode)BehaviorDesignerPreferences.GetInt(BDPreferences.GizmosViewMode);
			behavior.showBehaviorDesignerGizmo = BehaviorDesignerPreferences.GetBool(BDPreferences.ShowSceneIcon);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
		public static void HierarchyChange()
		{
			BehaviorManager instance = BehaviorManager.instance;
			if (Application.isPlaying)
			{
				if (instance != null)
				{
					instance.onEnableBehavior = new BehaviorManager.BehaviorManagerHandler(GizmoManager.UpdateBehaviorManagerGizmos);
				}
			}
			else
			{
				string name = SceneManager.GetActiveScene().name;
				if (GizmoManager.currentScene != name)
				{
					GizmoManager.currentScene = name;
					GizmoManager.UpdateAllGizmos();
				}
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000EE0C File Offset: 0x0000D00C
		private static void UpdateBehaviorManagerGizmos()
		{
			BehaviorManager instance = BehaviorManager.instance;
			if (instance != null)
			{
				for (int i = 0; i < instance.BehaviorTrees.Count; i++)
				{
					GizmoManager.UpdateGizmo(instance.BehaviorTrees[i].behavior);
				}
			}
		}

		// Token: 0x04000116 RID: 278
		private static string currentScene = SceneManager.GetActiveScene().name;
	}
}
