using System;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorDesigner.Editor
{
	// Token: 0x02000007 RID: 7
	public class BehaviorDesignerPreferences : UnityEditor.Editor
    {
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000029C8 File Offset: 0x00000BC8
		private static string[] PrefString
		{
			get
			{
				if (BehaviorDesignerPreferences.prefString == null)
				{
					BehaviorDesignerPreferences.InitPrefString();
				}
				return BehaviorDesignerPreferences.prefString;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000029E0 File Offset: 0x00000BE0
		public static void InitPrefernces()
		{
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[0]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ShowWelcomeScreen, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[1]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ShowSceneIcon, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[2]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ShowHierarchyIcon, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[3]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.OpenInspectorOnTaskSelection, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[3]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.OpenInspectorOnTaskSelection, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[5]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.FadeNodes, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[6]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.EditablePrefabInstances, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[7]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.PropertiesPanelOnLeft, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[8]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.MouseWhellScrolls, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[9]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.FoldoutFields, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[10]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.CompactMode, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[11]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.SnapToGrid, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[12]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ShowTaskDescription, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[13]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.BinarySerialization, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[14]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.ErrorChecking, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[15]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.SelectOnBreakpoint, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[16]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.UpdateCheck, true);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[17]))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.AddGameGUIComponent, false);
			}
			if (!EditorPrefs.HasKey(BehaviorDesignerPreferences.PrefString[18]))
			{
				BehaviorDesignerPreferences.SetInt(BDPreferences.GizmosViewMode, 2);
			}
			if (BehaviorDesignerPreferences.GetBool(BDPreferences.EditablePrefabInstances) && BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.BinarySerialization, false);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002BE8 File Offset: 0x00000DE8
		private static void InitPrefString()
		{
			BehaviorDesignerPreferences.prefString = new string[19];
			for (int i = 0; i < BehaviorDesignerPreferences.prefString.Length; i++)
			{
				BehaviorDesignerPreferences.prefString[i] = string.Format("BehaviorDesigner{0}", (BDPreferences)i);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002C30 File Offset: 0x00000E30
		public static void DrawPreferencesPane(PreferenceChangeHandler callback)
		{
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.ShowWelcomeScreen, "Show welcome screen", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.ShowSceneIcon, "Show Behavior Designer icon in the scene", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.ShowHierarchyIcon, "Show Behavior Designer icon in the hierarchy window", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.OpenInspectorOnTaskSelection, "Open inspector on single task selection", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.OpenInspectorOnTaskDoubleClick, "Open inspector on task double click", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.FadeNodes, "Fade tasks after they are done running", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.EditablePrefabInstances, "Allow edit of prefab instances", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.PropertiesPanelOnLeft, "Position properties panel on the left", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.MouseWhellScrolls, "Mouse wheel scrolls graph view", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.FoldoutFields, "Grouped fields start visible", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.CompactMode, "Compact mode", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.SnapToGrid, "Snap to grid", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.ShowTaskDescription, "Show selected task description", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.ErrorChecking, "Realtime error checking", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.SelectOnBreakpoint, "Select GameObject if a breakpoint is hit", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.UpdateCheck, "Check for updates", callback);
			BehaviorDesignerPreferences.DrawBoolPref(BDPreferences.AddGameGUIComponent, "Add Game GUI Component", callback);
			bool @bool = BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization);
			if (EditorGUILayout.Popup("Serialization", (!@bool) ? 1 : 0, BehaviorDesignerPreferences.serializationString, new GUILayoutOption[0]) != ((!@bool) ? 1 : 0))
			{
				BehaviorDesignerPreferences.SetBool(BDPreferences.BinarySerialization, !@bool);
				callback(BDPreferences.BinarySerialization, !@bool);
			}
			int @int = BehaviorDesignerPreferences.GetInt(BDPreferences.GizmosViewMode);
			int num = (int)((Behavior.GizmoViewMode)EditorGUILayout.EnumPopup("Gizmos View Mode", (Behavior.GizmoViewMode)@int, new GUILayoutOption[0]));
			if (num != @int)
			{
				BehaviorDesignerPreferences.SetInt(BDPreferences.GizmosViewMode, num);
				callback(BDPreferences.GizmosViewMode, num);
			}
			if (GUILayout.Button("Restore to Defaults", EditorStyles.miniButtonMid, new GUILayoutOption[0]))
			{
				BehaviorDesignerPreferences.ResetPrefs();
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002DCC File Offset: 0x00000FCC
		private static void DrawBoolPref(BDPreferences pref, string text, PreferenceChangeHandler callback)
		{
			bool @bool = BehaviorDesignerPreferences.GetBool(pref);
			bool flag = GUILayout.Toggle(@bool, text, new GUILayoutOption[0]);
			if (flag != @bool)
			{
				BehaviorDesignerPreferences.SetBool(pref, flag);
				callback(pref, flag);
				if (pref == BDPreferences.EditablePrefabInstances && flag && BehaviorDesignerPreferences.GetBool(BDPreferences.BinarySerialization))
				{
					BehaviorDesignerPreferences.SetBool(BDPreferences.BinarySerialization, false);
					callback(BDPreferences.BinarySerialization, false);
				}
				else if (pref == BDPreferences.BinarySerialization && flag && BehaviorDesignerPreferences.GetBool(BDPreferences.EditablePrefabInstances))
				{
					BehaviorDesignerPreferences.SetBool(BDPreferences.EditablePrefabInstances, false);
					callback(BDPreferences.EditablePrefabInstances, false);
				}
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002E6C File Offset: 0x0000106C
		private static void ResetPrefs()
		{
			BehaviorDesignerPreferences.SetBool(BDPreferences.ShowWelcomeScreen, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.ShowSceneIcon, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.ShowHierarchyIcon, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.OpenInspectorOnTaskSelection, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.OpenInspectorOnTaskDoubleClick, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.FadeNodes, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.EditablePrefabInstances, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.PropertiesPanelOnLeft, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.MouseWhellScrolls, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.FoldoutFields, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.CompactMode, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.SnapToGrid, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.ShowTaskDescription, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.BinarySerialization, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.ErrorChecking, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.SelectOnBreakpoint, false);
			BehaviorDesignerPreferences.SetBool(BDPreferences.UpdateCheck, true);
			BehaviorDesignerPreferences.SetBool(BDPreferences.AddGameGUIComponent, false);
			BehaviorDesignerPreferences.SetInt(BDPreferences.GizmosViewMode, 2);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002F08 File Offset: 0x00001108
		public static void SetBool(BDPreferences pref, bool value)
		{
			EditorPrefs.SetBool(BehaviorDesignerPreferences.PrefString[(int)pref], value);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002F18 File Offset: 0x00001118
		public static bool GetBool(BDPreferences pref)
		{
			return EditorPrefs.GetBool(BehaviorDesignerPreferences.PrefString[(int)pref]);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002F28 File Offset: 0x00001128
		public static void SetInt(BDPreferences pref, int value)
		{
			EditorPrefs.SetInt(BehaviorDesignerPreferences.PrefString[(int)pref], value);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002F38 File Offset: 0x00001138
		public static int GetInt(BDPreferences pref)
		{
			return EditorPrefs.GetInt(BehaviorDesignerPreferences.PrefString[(int)pref]);
		}

		// Token: 0x0400001D RID: 29
		private static string[] prefString;

		// Token: 0x0400001E RID: 30
		private static string[] serializationString = new string[]
		{
			"Binary",
			"JSON"
		};
	}
}
